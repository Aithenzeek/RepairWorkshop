using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using RepairWorkShop.DAL.Enums;
using System.Xml;

namespace RepairWorkshop.BLL.Services
{
    public class ServiceTaskService(AppDbContext context) : IServiceTaskService
    {
        public async Task<ResponseServiceTaskDto> CreateServiceTask(CreateServiceTaskDto dto)
        {
            var request = await context.Requests.Include(r => r.RepairItems).FirstOrDefaultAsync(r => r.RepairItems.Any(r => r.Id == dto.RepairItemId)) ?? throw new NotFoundException("Request not found");

            var existingRepairItem = await context.RepairItems.FindAsync(dto.RepairItemId) ?? throw new NotFoundException("Repair item not found");

            if (existingRepairItem.Status == RepairItemStatus.Completed ||
                existingRepairItem.Status == RepairItemStatus.Cancelled ||
                existingRepairItem.Status == RepairItemStatus.WaitingForPickUp)
                throw new ConflictException("Cant add to finished item");

            var existingServiceTask = await context.ServiceTasks.FirstOrDefaultAsync(s => s.ServiceId == dto.ServiceId);

            if (existingServiceTask != null && existingServiceTask.RepairItemId == dto.RepairItemId)
                throw new ConflictException("Task with same service exists");

            var service = await context.Services.FindAsync(dto.ServiceId) ?? throw new NotFoundException("Service not found");

            if (service.Status == ServiceStatus.Inactive)
                throw new ConflictException("Can`t add inactive service");

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
            var request = await context.Requests
                .Include(r => r.RepairItems)
                .ThenInclude(i => i.ServiceTasks)
                .FirstOrDefaultAsync(r => r.RepairItems.Any(i => i.ServiceTasks.Any(t => t.Id == id))) ?? throw new NotFoundException("Request not found");

            var serviceTask = request.RepairItems.SelectMany(s => s.ServiceTasks).FirstOrDefault(s => s.Id == id) ?? throw new NotFoundException("Service task not found");

            if (serviceTask.UserId != technicianId)
                throw new ConflictException("Cant do another technician work");

            if (serviceTask.Status != ServiceTaskStatus.New && serviceTask.Status != ServiceTaskStatus.OnHold && serviceTask.Status != ServiceTaskStatus.InProgress && serviceTask.Status != ServiceTaskStatus.WaitingForParts)
                throw new ConflictException("Service task must be new, on hold or wait for parts");

            serviceTask.Status = ServiceTaskStatus.InProgress;

            if (serviceTask.StartedAt == null)
                serviceTask.StartedAt = DateTime.Now;

            var repairItem = request.RepairItems.FirstOrDefault(r => r.Id == serviceTask.RepairItemId) ?? throw new NotFoundException("Repair item not found");

            repairItem.Status = RepairItemStatus.InProgress;

            if (repairItem.StartedAt == null) // TODO: переробити
                repairItem.StartedAt = serviceTask.StartedAt;

            request.Status = RequestStatus.InProgress;

            if (request.StartedAt == null)
                request.StartedAt = repairItem.StartedAt;

            await context.SaveChangesAsync();

            return ReturnDto(serviceTask);
        }

        public async Task<ResponseServiceTaskDto> CompleteServiceTask(CompleteServiceTaskDto dto, int technicianId)
        {
            var request = await context.Requests
                .Include(r => r.RepairItems)
                .ThenInclude(i => i.ServiceTasks)
                .FirstOrDefaultAsync(r => r.RepairItems.Any(i => i.ServiceTasks.Any(t => t.Id == dto.Id))) ?? throw new NotFoundException("Request not found");

            var serviceTask = request.RepairItems.SelectMany(s => s.ServiceTasks).FirstOrDefault(s => s.Id == dto.Id) ?? throw new NotFoundException("Service task not found");

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

            var repairItem = request.RepairItems.FirstOrDefault(r => r.Id == serviceTask.RepairItemId) ?? throw new NotFoundException("Repair item not found");

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
            //var serviceTask = await context.ServiceTasks.FindAsync(id);

            //if (serviceTask == null)
            //    throw new NotFoundException("Service task not found");

            //if (serviceTask.Status == ServiceTaskStatus.Draft)
            //    throw new ConflictException("Not allowed in draft");

            //serviceTask.Complete();

            //await context.SaveChangesAsync();

            //var repairItem = await context.RepairItems.FindAsync(serviceTask.RepairItemId);

            //var notCompletedServiceTask = await context.ServiceTasks.AnyAsync(s => s.RepairItemId == repairItem.Id && s.Status != ServiceTaskStatus.Completed);

            //if (!notCompletedServiceTask)
            //{
            //    repairItem.Complete();

            //    await context.SaveChangesAsync();

            //    var request = await context.Requests.Include(r => r.RepairItems).FirstOrDefaultAsync(r => r.Id == repairItem.CustomerRequestId);

            //    var notCompletedRepairItem = await context.RepairItems.AnyAsync(s => s.CustomerRequestId == request.Id && s.Status != RepairItemStatus.Completed);

            //    if (!notCompletedRepairItem)
            //        request.Complete();
            //}

            //await context.SaveChangesAsync();

            //return serviceTask;
        }

        public async Task<ResponseServiceTaskDto> CancelServiceTask(int id, int technicianId, CancelServiceTaskDto dto) // TODO: треба кенсел доробити нормально
        {
            var request = await context.Requests
                .Include(r => r.RepairItems)
                .ThenInclude(i => i.ServiceTasks)
                .FirstOrDefaultAsync(r => r.RepairItems.Any(i => i.ServiceTasks.Any(t => t.Id == id))) ?? throw new NotFoundException("Request not found");

            var serviceTask = request.RepairItems.SelectMany(s => s.ServiceTasks).FirstOrDefault(s => s.Id == id) ?? throw new NotFoundException("Service task not found");

            if (serviceTask.UserId != technicianId)
                throw new ConflictException("Cant do another technician work");

            if (serviceTask.Status == ServiceTaskStatus.Draft || serviceTask.Status == ServiceTaskStatus.Completed)
                throw new ConflictException("Can`t cancel draft or completed service task");

            serviceTask.Cancel();

            serviceTask.CancellationReason = dto.CancellationReason;

            var repairItem = request.RepairItems.FirstOrDefault(r => r.Id == serviceTask.RepairItemId) ?? throw new NotFoundException("Repair item not found");

            if (repairItem.ServiceTasks.All(s => s.Status == ServiceTaskStatus.Draft || s.Status == ServiceTaskStatus.Cancelled))
                repairItem.SetOnHold();

            if (request.RepairItems.All(r => r.Status == RepairItemStatus.Draft || r.Status == RepairItemStatus.Cancelled || r.Status == RepairItemStatus.OnHold))
                request.SetOnHold();

            await context.SaveChangesAsync();

            return ReturnDto(serviceTask);
        }

        public async Task<ResponseServiceTaskDto> SetOnHoldServiceTask(int id, int technicianId)
        {
            var request = await context.Requests
                .Include(r => r.RepairItems)
                .ThenInclude(i => i.ServiceTasks)
                .FirstOrDefaultAsync(r => r.RepairItems.Any(i => i.ServiceTasks.Any(t => t.Id == id))) ?? throw new NotFoundException("Request not found");

            var serviceTask = request.RepairItems.SelectMany(s => s.ServiceTasks).FirstOrDefault(s => s.Id == id) ?? throw new NotFoundException("Service task not found");

            if (serviceTask.UserId != technicianId)
                throw new ConflictException("Cant do another technician work");

            if (serviceTask.Status != ServiceTaskStatus.InProgress && serviceTask.Status != ServiceTaskStatus.WaitingForParts && serviceTask.Status != ServiceTaskStatus.OnHold)
                throw new ConflictException("Only in progress");

            if (serviceTask.Status == ServiceTaskStatus.Draft &&
                serviceTask.Status == ServiceTaskStatus.New &&
                serviceTask.Status == ServiceTaskStatus.WaitingForParts &&
                serviceTask.Status == ServiceTaskStatus.Cancelled &&
                serviceTask.Status == ServiceTaskStatus.Completed
                )
                throw new ConflictException("Not allowed in this status");

            serviceTask.SetOnHold();

            var repairItem = request.RepairItems.FirstOrDefault(r => r.Id == serviceTask.RepairItemId) ?? throw new NotFoundException("Repair item not found");

            if (repairItem.Status != RepairItemStatus.Draft &&
                repairItem.Status != RepairItemStatus.New &&
                repairItem.Status != RepairItemStatus.Completed &&
                repairItem.Status != RepairItemStatus.Cancelled &&
                repairItem.Status != RepairItemStatus.WaitingForPickUp
                )
                if (repairItem.ServiceTasks.All(s => s.Status == ServiceTaskStatus.OnHold || s.Status == ServiceTaskStatus.Cancelled || s.Status == ServiceTaskStatus.Completed || s.Status == ServiceTaskStatus.Draft || s.Status == ServiceTaskStatus.New))
                    repairItem.SetOnHold();

            if (repairItem.Status != RepairItemStatus.Draft &&
                request.Status != RequestStatus.New &&
                request.Status != RequestStatus.Completed &&
                request.Status != RequestStatus.Cancelled &&
                request.Status != RequestStatus.WaitingForPickUp &&
                request.Status != RequestStatus.PickedUp
                )
                if (request.RepairItems.All(r => r.Status == RepairItemStatus.OnHold || r.Status == RepairItemStatus.Completed || r.Status == RepairItemStatus.Cancelled || r.Status == RepairItemStatus.New || r.Status == RepairItemStatus.Draft))
                    request.SetOnHold();

            await context.SaveChangesAsync();

            return ReturnDto(serviceTask);

            //var serviceTask = await context.ServiceTasks.FindAsync(id);

            //if (serviceTask == null)
            //    throw new NotFoundException("Service task not found");

            //if (serviceTask.Status == ServiceTaskStatus.Draft)
            //    throw new ConflictException("Not allowed in draft");

            //serviceTask.SetOnHold();

            //await context.SaveChangesAsync();

            //return serviceTask;
        }

        public async Task<ResponseServiceTaskDto> WaitForServiceTaskParts(int id, int technicianId)
        {
            var request = await context.Requests
                .Include(r => r.RepairItems)
                .ThenInclude(i => i.ServiceTasks)
                .FirstOrDefaultAsync(r => r.RepairItems.Any(i => i.ServiceTasks.Any(t => t.Id == id))) ?? throw new NotFoundException("request not found");

            var serviceTask = request.RepairItems.SelectMany(s => s.ServiceTasks).FirstOrDefault(s => s.Id == id) ?? throw new NotFoundException("Service task not found");

            if (serviceTask.Status != ServiceTaskStatus.InProgress && serviceTask.Status != ServiceTaskStatus.OnHold && serviceTask.Status != ServiceTaskStatus.WaitingForParts)
                throw new ConflictException("Allowed in progress or on hold");

            if (serviceTask.Status == ServiceTaskStatus.Draft &&
                serviceTask.Status == ServiceTaskStatus.New &&
                serviceTask.Status == ServiceTaskStatus.OnHold &&
                serviceTask.Status == ServiceTaskStatus.Cancelled &&
                serviceTask.Status == ServiceTaskStatus.Completed
                )
                throw new ConflictException("Not allowed in this status");

            serviceTask.WaitForParts();

            var repairItem = request.RepairItems.FirstOrDefault(r => r.Id == serviceTask.RepairItemId) ?? throw new NotFoundException("Repair item not found");

            if (repairItem.Status != RepairItemStatus.Draft &&
                repairItem.Status != RepairItemStatus.New &&
                repairItem.Status != RepairItemStatus.Completed &&
                repairItem.Status != RepairItemStatus.Cancelled &&
                repairItem.Status != RepairItemStatus.WaitingForPickUp
                )
                if (repairItem.ServiceTasks.All(s => s.Status == ServiceTaskStatus.WaitingForParts || s.Status == ServiceTaskStatus.Cancelled || s.Status == ServiceTaskStatus.Completed || s.Status == ServiceTaskStatus.OnHold))
                    repairItem.WaitForParts();

            if (repairItem.Status != RepairItemStatus.Draft &&
                request.Status != RequestStatus.New &&
                request.Status != RequestStatus.Completed &&
                request.Status != RequestStatus.Cancelled &&
                request.Status != RequestStatus.WaitingForPickUp &&
                request.Status != RequestStatus.PickedUp
                )
                if (request.RepairItems.All(r => r.Status == RepairItemStatus.WaitingForParts || r.Status == RepairItemStatus.Cancelled || r.Status == RepairItemStatus.Completed || r.Status == RepairItemStatus.Draft))
                    request.WaitForParts();

            await context.SaveChangesAsync();

            return ReturnDto(serviceTask);

            //var serviceTask = await context.ServiceTasks.FindAsync(id);

            //if (serviceTask == null)
            //    throw new NotFoundException("Service task not found");

            //if (serviceTask.Status == ServiceTaskStatus.Draft)
            //    throw new ConflictException("Not allowed in draft");

            //serviceTask.Status = ServiceTaskStatus.WaitingForParts;

            //await context.SaveChangesAsync();

            //return serviceTask;
        }

        public async Task<ResponseServiceTaskDto> EditServiceTask(int id, EditServiceTaskDto dto)
        {
            var serviceTask = await context.ServiceTasks.Include(s => s.Service).FirstOrDefaultAsync(s => s.Id == id) ?? throw new NotFoundException("Service task not found"); //TODO статуси: в процесі і чернетка. Зробити шоб повтора не було

            var service = await context.Services.FindAsync(dto.ServiceId) ?? throw new NotFoundException("Service not found");

            if (serviceTask.Service.Status == ServiceStatus.Active && service.Status == ServiceStatus.Inactive)
                throw new ConflictException("Can`t add inactive service");

            if (serviceTask.Status != ServiceTaskStatus.Draft)
                throw new ConflictException("Can be edit only in draft");

            serviceTask.UserId = dto.UserId;
            serviceTask.ServiceId = dto.ServiceId;

            //if ((serviceTask.Status != ServiceTaskStatus.Completed
            //    && serviceTask.Status != ServiceTaskStatus.Cancelled
            //    && serviceTask.Status != ServiceTaskStatus.Draft) && serviceTask.ServiceId == dto.ServiceId)
            //    serviceTask.UserId = dto.UserId;

            await context.SaveChangesAsync();

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

        public async Task<List<ServiceTask>> GetAllServiceTasks()
        {
            return await context.ServiceTasks
                .AsNoTracking()
                .Include(s => s.Service)
                .Include(s => s.User)
                .ToListAsync();
        }

        public async Task<List<ServiceTask>> GetAllActiveServiceTasks(int userId, bool activeOnly)
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
                serviceTask.DiagnosticsResult = serviceTask.DiagnosticsResult
            );
        }
    }
}
