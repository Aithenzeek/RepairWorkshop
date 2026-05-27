using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using RepairWorkShop.DAL.Enums;

namespace RepairWorkshop.BLL.Services
{
    public class ServiceTaskService(AppDbContext context) : IServiceTaskService
    {
        public async Task<ResponseServiceTaskDto> CreateServiceTask(CreateServiceTaskDto dto)
        {
            var request = await context.Requests
                .Include(r => r.RepairItems)
                .FirstOrDefaultAsync(r => r.RepairItems
                .Any(r => r.Id == dto.RepairItemId)) ?? throw new NotFoundException("Request not found");

            var existingRepairItem = await context.RepairItems
                .Include(r => r.ServiceTasks)
                .FirstOrDefaultAsync(r => r.Id == dto.RepairItemId) ?? throw new NotFoundException("Repair item not found");

            if (existingRepairItem.Status == RepairItemStatus.Completed ||
                existingRepairItem.Status == RepairItemStatus.Cancelled ||
                existingRepairItem.Status == RepairItemStatus.WaitingForPickUp ||
                existingRepairItem.Status == RepairItemStatus.PickedUp)
                throw new ConflictException("Can`t add to finished item");

            var existingServiceTask = await context.ServiceTasks.FirstOrDefaultAsync(s => s.ServiceId == dto.ServiceId);

            if (existingServiceTask != null && existingServiceTask.RepairItemId == dto.RepairItemId)
                throw new ConflictException("Task with same service exists");

            var service = await context.Services.FindAsync(dto.ServiceId) ?? throw new NotFoundException("Service not found");

            if (service.Status == ServiceStatus.Inactive)
                throw new ConflictException("Can`t add inactive service");

            if (service.Name != "Diagnostics" && !existingRepairItem.ServiceTasks.Any())
                throw new ConflictException("First item task must be diagnostics");

            var user = await context.Users.Include(r => r.Role).FirstOrDefaultAsync(u => u.Id == dto.UserId);

            if (user != null && user.Role.Name != "Technician")
                throw new BadRequestException("Selected user role is not technician");

            var serviceTask = new ServiceTask
            {
                RepairItemId = dto.RepairItemId,
                UserId = dto.UserId,
                ServiceId = dto.ServiceId,
                Cost = service.Price,
                Status = ServiceTaskStatus.Draft
            };

            if (existingRepairItem.Status == RepairItemStatus.CompletedByTechnician)
                existingRepairItem.Status = RepairItemStatus.OnHold;

            if (request.Status == RequestStatus.CompletedByTechnician)
                request.Status = RequestStatus.OnHold;

            await context.ServiceTasks.AddAsync(serviceTask);
            await context.SaveChangesAsync();

            return ReturnDto(serviceTask);
        }

        public async Task DeleteServiceTask(int id)
        {
            var serviceTask = await context.ServiceTasks.FindAsync(id) ?? throw new NotFoundException("Service task not found");

            if (serviceTask.Status != ServiceTaskStatus.Draft)
                throw new ConflictException("Only draft can be deleted");

            context.ServiceTasks.Remove(serviceTask);
            await context.SaveChangesAsync();
        }

        public async Task<ResponseServiceTaskDto> StartServiceTask(int id, int technicianId)
        {
            var request = await GetRequest(id);

            var serviceTask = request.RepairItems
                .SelectMany(s => s.ServiceTasks)
                .FirstOrDefault(s => s.Id == id) ?? throw new NotFoundException("Service task not found");

            if (serviceTask.UserId != technicianId)
                throw new ConflictException("Cant do another technician work");

            if (serviceTask.Status != ServiceTaskStatus.New &&
                serviceTask.Status != ServiceTaskStatus.OnHold &&
                serviceTask.Status != ServiceTaskStatus.InProgress &&
                serviceTask.Status != ServiceTaskStatus.WaitingForParts)
                throw new ConflictException("Service task must be started");

            serviceTask.Status = ServiceTaskStatus.InProgress;

            if (serviceTask.StartedAt == null)
                serviceTask.StartedAt = DateTime.Now;

            var repairItem = request.RepairItems
                .FirstOrDefault(r => r.Id == serviceTask.RepairItemId) ?? throw new NotFoundException("Repair item not found");

            repairItem.Status = RepairItemStatus.InProgress;

            if (repairItem.StartedAt == null)
                repairItem.StartedAt = serviceTask.StartedAt;

            request.Status = RequestStatus.InProgress;

            if (request.StartedAt == null)
                request.StartedAt = repairItem.StartedAt;

            await context.SaveChangesAsync();

            return ReturnDto(serviceTask);
        }

        public async Task<ResponseServiceTaskDto> CompleteServiceTask(CompleteServiceTaskDto dto, int technicianId)
        {
            var request = await GetRequest(dto.Id);

            var serviceTask = request.RepairItems
                .SelectMany(s => s.ServiceTasks)
                .FirstOrDefault(s => s.Id == dto.Id) ?? throw new NotFoundException("Service task not found");

            if (serviceTask.UserId != technicianId)
                throw new ConflictException("Cant do another technician work");

            if (serviceTask.Status == ServiceTaskStatus.Draft ||
                serviceTask.Status == ServiceTaskStatus.New ||
                serviceTask.Status == ServiceTaskStatus.WaitingForParts ||
                serviceTask.Status == ServiceTaskStatus.Cancelled
                )
                throw new ConflictException("Not allowed in this status");

            if (context.Services.Any(s => s.Name == "Diagnostics" && s.Id == serviceTask.ServiceId))
                serviceTask.DiagnosticsResult = dto.DiagnosticsResult;

            serviceTask.Complete();

            var repairItem = request.RepairItems
                .FirstOrDefault(r => r.Id == serviceTask.RepairItemId) ?? throw new NotFoundException("Repair item not found");

            if (repairItem.Status != RepairItemStatus.Draft &&
                repairItem.Status != RepairItemStatus.New &&
                repairItem.Status != RepairItemStatus.WaitingForParts &&
                repairItem.Status != RepairItemStatus.Cancelled &&
                repairItem.Status != RepairItemStatus.WaitingForPickUp
                )
                if (repairItem.ServiceTasks.All(s => s.Status == ServiceTaskStatus.Completed || s.Status == ServiceTaskStatus.Cancelled))
                {
                    repairItem.CompleteByTechnician();
                    repairItem.GetServiceCost();

                    if (repairItem.Status != RepairItemStatus.Draft &&
                        request.Status != RequestStatus.New &&
                        request.Status != RequestStatus.Cancelled &&
                        request.Status != RequestStatus.WaitingForPickUp &&
                        request.Status != RequestStatus.PickedUp
                        )
                        if (request.RepairItems.All(r => r.Status == RepairItemStatus.CompletedByTechnician))
                        {
                            request.CompleteByTechnician();
                            request.GetTotalCost();
                        }
                }

            await context.SaveChangesAsync();

            return ReturnDto(serviceTask);
        }

        public async Task<ResponseServiceTaskDto> CancelServiceTask(int id, int technicianId, CancelServiceTaskDto dto)
        {
            var request = await GetRequest(id);

            var serviceTask = request.RepairItems
                .SelectMany(s => s.ServiceTasks)
                .FirstOrDefault(s => s.Id == id) ?? throw new NotFoundException("Service task not found");

            if (serviceTask.Status == ServiceTaskStatus.Draft || serviceTask.Status == ServiceTaskStatus.Completed)
                throw new ConflictException("Can`t cancel draft or completed service task");

            serviceTask.Cancel();

            serviceTask.CancellationReason = dto.CancellationReason;

            var repairItem = request.RepairItems
                .FirstOrDefault(r => r.Id == serviceTask.RepairItemId) ?? throw new NotFoundException("Repair item not found");

            if (repairItem.ServiceTasks.All(s => s.Status == ServiceTaskStatus.Draft || s.Status == ServiceTaskStatus.Cancelled))
                repairItem.SetOnHold();

            if (request.RepairItems
                .All(r => r.Status == RepairItemStatus.Draft ||
                r.Status == RepairItemStatus.Cancelled ||
                r.Status == RepairItemStatus.OnHold))
                request.SetOnHold();

            await context.SaveChangesAsync();

            return ReturnDto(serviceTask);
        }

        public async Task<ResponseServiceTaskDto> SetOnHoldServiceTask(int id, int technicianId)
        {
            var request = await GetRequest(id);

            var serviceTask = request.RepairItems
                .SelectMany(s => s.ServiceTasks)
                .FirstOrDefault(s => s.Id == id) ?? throw new NotFoundException("Service task not found");

            if (serviceTask.UserId != technicianId)
                throw new ConflictException("Cant do another technician work");

            if (serviceTask.Status != ServiceTaskStatus.InProgress &&
                serviceTask.Status != ServiceTaskStatus.WaitingForParts &&
                serviceTask.Status != ServiceTaskStatus.OnHold)
                throw new ConflictException("Only in progress, waiting for parts or on hold");

            if (serviceTask.Status == ServiceTaskStatus.Draft &&
                serviceTask.Status == ServiceTaskStatus.New &&
                serviceTask.Status == ServiceTaskStatus.Cancelled &&
                serviceTask.Status == ServiceTaskStatus.Completed
                )
                throw new ConflictException("Not allowed in this status");

            serviceTask.SetOnHold();

            var repairItem = request.RepairItems
                .FirstOrDefault(r => r.Id == serviceTask.RepairItemId) ?? throw new NotFoundException("Repair item not found");

            if (repairItem.Status != RepairItemStatus.Draft &&
                repairItem.Status != RepairItemStatus.New &&
                repairItem.Status != RepairItemStatus.Completed &&
                repairItem.Status != RepairItemStatus.Cancelled &&
                repairItem.Status != RepairItemStatus.WaitingForPickUp
                )
                if (repairItem.ServiceTasks.All(s => s.Status == ServiceTaskStatus.OnHold ||
                s.Status == ServiceTaskStatus.Cancelled ||
                s.Status == ServiceTaskStatus.Completed ||
                s.Status == ServiceTaskStatus.Draft ||
                s.Status == ServiceTaskStatus.New))
                    repairItem.SetOnHold();

            if (repairItem.Status != RepairItemStatus.Draft &&
                request.Status != RequestStatus.New &&
                request.Status != RequestStatus.Completed &&
                request.Status != RequestStatus.Cancelled &&
                request.Status != RequestStatus.WaitingForPickUp &&
                request.Status != RequestStatus.PickedUp
                )
                if (request.RepairItems.All(r => r.Status == RepairItemStatus.OnHold ||
                r.Status == RepairItemStatus.Completed ||
                r.Status == RepairItemStatus.Cancelled ||
                r.Status == RepairItemStatus.New ||
                r.Status == RepairItemStatus.Draft))
                    request.SetOnHold();

            await context.SaveChangesAsync();

            return ReturnDto(serviceTask);
        }

        public async Task<ResponseServiceTaskDto> WaitForServiceTaskParts(int id, int technicianId)
        {
            var request = await context.Requests
                .Include(r => r.RepairItems)
                    .ThenInclude(i => i.ServiceTasks)
                        .ThenInclude(s => s.User)
                 .Include(r => r.RepairItems)
                    .ThenInclude(i => i.ServiceTasks)
                        .ThenInclude(s => s.Service)
                .FirstOrDefaultAsync(r => r.RepairItems
                .Any(i => i.ServiceTasks
                .Any(t => t.Id == id))) ?? throw new NotFoundException("request not found");

            var serviceTask = request.RepairItems
                .SelectMany(s => s.ServiceTasks)
                .FirstOrDefault(s => s.Id == id) ?? throw new NotFoundException("Service task not found");

            if (serviceTask.Status != ServiceTaskStatus.InProgress &&
                serviceTask.Status != ServiceTaskStatus.OnHold &&
                serviceTask.Status != ServiceTaskStatus.WaitingForParts)
                throw new ConflictException("Allowed in progress or on hold");

            if (serviceTask.Status == ServiceTaskStatus.Draft &&
                serviceTask.Status == ServiceTaskStatus.New &&
                serviceTask.Status == ServiceTaskStatus.OnHold &&
                serviceTask.Status == ServiceTaskStatus.Cancelled &&
                serviceTask.Status == ServiceTaskStatus.Completed
                )
                throw new ConflictException("Not allowed in this status");

            serviceTask.WaitForParts();

            var repairItem = request.RepairItems
                .FirstOrDefault(r => r.Id == serviceTask.RepairItemId) ?? throw new NotFoundException("Repair item not found");

            if (repairItem.Status != RepairItemStatus.Draft &&
                repairItem.Status != RepairItemStatus.New &&
                repairItem.Status != RepairItemStatus.Completed &&
                repairItem.Status != RepairItemStatus.Cancelled &&
                repairItem.Status != RepairItemStatus.WaitingForPickUp
                )
                if (repairItem.ServiceTasks
                    .All(s => s.Status == ServiceTaskStatus.WaitingForParts ||
                    s.Status == ServiceTaskStatus.Cancelled ||
                    s.Status == ServiceTaskStatus.Completed ||
                    s.Status == ServiceTaskStatus.OnHold))
                    repairItem.WaitForParts();

            if (repairItem.Status != RepairItemStatus.Draft &&
                request.Status != RequestStatus.New &&
                request.Status != RequestStatus.Completed &&
                request.Status != RequestStatus.Cancelled &&
                request.Status != RequestStatus.WaitingForPickUp &&
                request.Status != RequestStatus.PickedUp
                )
                if (request.RepairItems.All(r => r.Status == RepairItemStatus.WaitingForParts ||
                r.Status == RepairItemStatus.Cancelled ||
                r.Status == RepairItemStatus.Completed ||
                r.Status == RepairItemStatus.Draft))
                    request.WaitForParts();

            await context.SaveChangesAsync();

            return ReturnDto(serviceTask);
        }

        public async Task<ResponseServiceTaskDto> EditServiceTask(int id, EditServiceTaskDto dto)
        {
            var repairItem = await context.RepairItems
                .Include(r => r.ServiceTasks)
                    .ThenInclude(r => r.User)
                .Include(r => r.ServiceTasks)
                    .ThenInclude(r => r.Service)
                .FirstOrDefaultAsync(r => r.ServiceTasks.Any(s => s.Id == id)) ?? throw new NotFoundException("Repair item not found");

            var serviceTask = repairItem.ServiceTasks.FirstOrDefault(s => s.Id == id) ?? throw new NotFoundException("Service task not found");

            var service = await context.Services.FindAsync(dto.ServiceId) ?? throw new NotFoundException("Service not found");

            var existingServiceTask = await context.ServiceTasks.FirstOrDefaultAsync(r => r.ServiceId == dto.ServiceId);

            if (serviceTask.Service.Status == ServiceStatus.Active && service.Status == ServiceStatus.Inactive)
                throw new ConflictException("Can`t set inactive service");

            if (serviceTask.Status != ServiceTaskStatus.Draft)
                throw new ConflictException("Can be edit only in draft");

            if (serviceTask != null && existingServiceTask != null && dto.ServiceId == existingServiceTask.ServiceId)
                throw new ConflictException("Task with same service exists");

            serviceTask.UserId = dto.UserId;
            serviceTask.ServiceId = dto.ServiceId;

            await context.SaveChangesAsync();

            serviceTask = await context.ServiceTasks
                .Include(s => s.User)
                .Include(s => s.Service)
                .FirstOrDefaultAsync(s => s.Id == id) ?? throw new NotFoundException("Service task not found");

            return ReturnDto(serviceTask);
        }

        public async Task<ServiceTask?> GetServiceTaskById(int id)
        {
            return await context.ServiceTasks
                .AsNoTracking()
                .Include(s => s.Service)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id) ?? throw new NotFoundException("Service task not found");
        }

        public async Task<List<ServiceTask>> GetAllServiceTasks() // не використовується
        {
            return await context.ServiceTasks
                .AsNoTracking()
                .Include(s => s.Service)
                .Include(s => s.User)
                .ToListAsync();
        }

        public async Task<List<ServiceTask>> GetAllActiveServiceTasks(int userId, bool activeOnly) // не використовується
        {
            var serviceTasks = context.ServiceTasks.Where(t => t.UserId == userId);

            if (activeOnly)
            {
                serviceTasks = serviceTasks
                    .Include(s => s.Service)
                    .Include(s => s.User)
                    .Where(t =>
                        t.Status != ServiceTaskStatus.Draft &&
                        t.Status != ServiceTaskStatus.Completed &&
                        t.Status != ServiceTaskStatus.Cancelled);
            }

            return await serviceTasks.ToListAsync();
        }

        public ResponseServiceTaskDto ReturnDto(ServiceTask serviceTask)
        {
            return new ResponseServiceTaskDto(
                serviceTask.Id = serviceTask.Id,
                serviceTask.RepairItemId = serviceTask.RepairItemId,
                serviceTask.UserId = serviceTask.UserId,
                serviceTask.ServiceId = serviceTask.ServiceId,
                serviceTask.Cost = serviceTask.Cost,
                serviceTask.Status = serviceTask.Status,
                serviceTask.StartedAt = serviceTask.StartedAt,
                serviceTask.CompletedAt = serviceTask.CompletedAt,
                serviceTask.DiagnosticsResult = serviceTask.DiagnosticsResult,
                serviceTask.User.Name,
                serviceTask.Service.Name,
                serviceTask.CancellationReason
            );
        }

        public async Task<PagedResponse<ResponseServiceTaskDto>> GetPaged(ServiceTaskFilterDto filter)
        {
            var query = context.ServiceTasks
                .Include(x => x.User)
                .Include(x => x.Service)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                query = query.Where(x =>
                    x.Status.ToString() == filter.Status);
            }

            if (filter.DateFrom.HasValue)
            {
                query = query.Where(x =>
                    x.StartedAt >= filter.DateFrom.Value);
            }

            if (filter.DateTo.HasValue)
            {
                query = query.Where(x =>
                    x.StartedAt <= filter.DateTo.Value);
            }

            if (filter.RepairItemId.HasValue)
            {
                query = query.Where(x =>
                    x.RepairItemId == filter.RepairItemId.Value);
            }

            query = filter.SortBy?.ToLower() switch
            {
                "dateasc" => query.OrderBy(x => x.StartedAt),
                "datedesc" => query.OrderByDescending(x => x.StartedAt),

                "status" => query.OrderBy(x => x.Status),

                "costasc" => query.OrderBy(x => x.Cost),
                "costdesc" => query.OrderByDescending(x => x.Cost),

                _ => query.OrderByDescending(x => x.StartedAt)
            };

            var total = await query.CountAsync();

            var items = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new ResponseServiceTaskDto(
                    x.Id,
                    x.RepairItemId,
                    x.UserId,
                    x.ServiceId,
                    x.Cost,
                    x.Status,
                    x.StartedAt,
                    x.CompletedAt,
                    x.DiagnosticsResult,
                    x.User.Name,
                    x.Service.Name,
                    x.CancellationReason
                ))
                .ToListAsync();

            return new PagedResponse<ResponseServiceTaskDto>
            {
                Items = items,
                TotalCount = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<PagedResponse<ResponseServiceTaskDto>> GetActivePaged(int userId, bool activeOnly, int page = 1, int pageSize = 10)
        {
            var query = context.ServiceTasks.Where(t => t.UserId == userId);

            if (activeOnly)
            {
                query = query
                    .Include(s => s.Service)
                    .Include(s => s.User)
                    .Where(t =>
                        t.Status != ServiceTaskStatus.Draft &&
                        t.Status != ServiceTaskStatus.Completed &&
                        t.Status != ServiceTaskStatus.Cancelled);
            }

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ResponseServiceTaskDto(
                    x.Id,
                    x.RepairItemId,
                    x.UserId,
                    x.ServiceId,
                    x.Cost,
                    x.Status,
                    x.StartedAt,
                    x.CompletedAt,
                    x.DiagnosticsResult,
                    x.User.Name,
                    x.Service.Name,
                    x.CancellationReason
                ))
                .ToListAsync();

            return new PagedResponse<ResponseServiceTaskDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        private async Task<CustomerRequest> GetRequest(int id)
        {
            return await context.Requests
                .Include(r => r.RepairItems)
                    .ThenInclude(i => i.ServiceTasks)
                        .ThenInclude(s => s.User)
                 .Include(r => r.RepairItems)
                    .ThenInclude(i => i.ServiceTasks)
                        .ThenInclude(s => s.Service)
                .FirstOrDefaultAsync(r => r.RepairItems
                .Any(i => i.ServiceTasks
                .Any(t => t.Id == id))) ?? throw new NotFoundException("Request not found");
        }
    }
}
