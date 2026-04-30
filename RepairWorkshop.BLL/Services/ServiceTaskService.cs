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
        public async Task<ServiceTask> CreateServiceTask(CreateServiceTaskDto dto)
        {
            var existingServiceTask = await context.ServiceTasks.FindAsync(dto.ServiceId);

            if (existingServiceTask != null && existingServiceTask.RepairItemId == dto.RepairItemId)
                throw new ConflictException("Task with same service exists");

            var service = await context.Services.FindAsync(dto.ServiceId);

            if (service == null)
                throw new NotFoundException("Service not found");

            if (service.Status == ServiceStatus.Inactive)
                throw new ConflictException("Can`t add inactive service");

            var existingRepairItem = await context.RepairItems.FindAsync(dto.RepairItemId);

            if (existingRepairItem == null)
                throw new NotFoundException("repair item not found");

            var serviceTask = new ServiceTask
            {
                RepairItemId = dto.RepairItemId,
                UserId = dto.UserId,
                ServiceId = dto.ServiceId,
                Cost = service.Price,
                Status = ServiceTaskStatus.Draft
            };

            if (existingRepairItem.Status == RepairItemStatus.CompletedByTechnician)
                existingRepairItem.Status = RepairItemStatus.InProgress;

            await context.ServiceTasks.AddAsync(serviceTask);
            await context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task DeleteServiceTask(int id)
        {
            var serviceTask = await context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new NotFoundException("Service task not found");

            if (serviceTask.Status != ServiceTaskStatus.Draft)
                throw new ConflictException("Only draft can be deleted");

            context.ServiceTasks.Remove(serviceTask);
            await context.SaveChangesAsync();
        }

        public async Task<ServiceTask> StartServiceTask(int id)
        {
            var request = await context.Requests
                .Include(r => r.RepairItems)
                .ThenInclude(i => i.ServiceTasks)
                .FirstOrDefaultAsync(r => r.RepairItems.Any(i => i.ServiceTasks.Any(t => t.Id == id)));

            if (request == null)
                throw new NotFoundException("Request not found");

            var serviceTask = request.RepairItems.SelectMany(s => s.ServiceTasks).FirstOrDefault(s => s.Id == id);

            if (serviceTask == null)
                throw new NotFoundException("Service task not found");

            if (serviceTask.Status != ServiceTaskStatus.Draft)
                throw new ConflictException("Allowed only in draft");

            serviceTask.Status = ServiceTaskStatus.New;
            serviceTask.StartedAt = DateTime.Now;

            var repairItem = request.RepairItems.FirstOrDefault(r => r.Id == serviceTask.RepairItemId);

            if (repairItem == null)
                throw new NotFoundException("Repair item not found");

            if (repairItem.StartedAt == null) // TODO: переробити
            {
                repairItem.Status = RepairItemStatus.New;
                repairItem.StartedAt = serviceTask.StartedAt;
            }

            if (request.StartedAt == null)
            {
                request.Status = RequestStatus.New;
                request.StartedAt = repairItem.StartedAt;
            }

            await context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask> CompleteServiceTask(CompleteServiceTaskDto dto)
        {
            var request = await context.Requests
                .Include(r => r.RepairItems)
                .ThenInclude(i => i.ServiceTasks)
                .FirstOrDefaultAsync(r => r.RepairItems.Any(i => i.ServiceTasks.Any(t => t.Id == dto.id)));

            if (request == null)
                throw new NotFoundException("Request not found");

            var serviceTask = request.RepairItems.SelectMany(s => s.ServiceTasks).FirstOrDefault(s => s.Id == dto.id);

            if (serviceTask == null)
                throw new NotFoundException("Service task not found");

            if (serviceTask.Status == ServiceTaskStatus.Draft &&
                serviceTask.Status == ServiceTaskStatus.New &&
                serviceTask.Status == ServiceTaskStatus.WaitingForParts &&
                serviceTask.Status == ServiceTaskStatus.Cancelled
                )
                throw new ConflictException("Not allowed in this status");

            if (context.Services.Any(s => s.Name == "Diagnostics" && s.Id == serviceTask.ServiceId))
                serviceTask.DiagnosticsResult = dto.DiagnosticsResult;

            serviceTask.Complete();

            var repairItem = request.RepairItems.FirstOrDefault(r => r.Id == serviceTask.RepairItemId);

            if (repairItem == null)
                throw new NotFoundException("Repair item not found");

            if (repairItem.Status != RepairItemStatus.Draft &&
                repairItem.Status != RepairItemStatus.New &&
                repairItem.Status != RepairItemStatus.WaitingForParts &&
                repairItem.Status != RepairItemStatus.Cancelled &&
                repairItem.Status != RepairItemStatus.WaitingForPickUp
                )
                if (repairItem.ServiceTasks.All(s => s.Status == ServiceTaskStatus.Completed || s.Status == ServiceTaskStatus.Cancelled))
                {
                    repairItem.Complete();
                    repairItem.GetServiceCost();

                    if (repairItem.Status != RepairItemStatus.Draft &&
                        request.Status != RequestStatus.New &&
                        request.Status != RequestStatus.Cancelled &&
                        request.Status != RequestStatus.WaitingForPickUp &&
                        request.Status != RequestStatus.PickedUp
                        )
                        if (request.RepairItems.All(r => r.Status == RepairItemStatus.CompletedByTechnician))
                        {
                            request.Complete();
                            request.GetTotalCost();
                        }
                }

            await context.SaveChangesAsync();

            return serviceTask;
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

        public async Task<ServiceTask> CancelServiceTask(int id) // TODO: треба кенсел доробити нормально
        {
            var serviceTask = await context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new NotFoundException("Service task not found");

            if (serviceTask.Status == ServiceTaskStatus.Draft && serviceTask.Status == ServiceTaskStatus.Completed)
                throw new ConflictException("Can`t cancel draft or completed service task");

            serviceTask.Cancel();

            await context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask> SetOnHoldServiceTask(int id)
        {
            var request = await context.Requests
                .Include(r => r.RepairItems)
                .ThenInclude(i => i.ServiceTasks)
                .FirstOrDefaultAsync(r => r.RepairItems.Any(i => i.ServiceTasks.Any(t => t.Id == id)));

            if (request == null)
                throw new NotFoundException("request not found");

            var serviceTask = request.RepairItems.SelectMany(s => s.ServiceTasks).FirstOrDefault(s => s.Id == id);

            if (serviceTask == null)
                throw new NotFoundException("Service task not found");

            if (serviceTask.Status == ServiceTaskStatus.Draft &&
                serviceTask.Status == ServiceTaskStatus.New &&
                serviceTask.Status == ServiceTaskStatus.WaitingForParts &&
                serviceTask.Status == ServiceTaskStatus.Cancelled &&
                serviceTask.Status == ServiceTaskStatus.Completed
                )
                throw new ConflictException("Not allowed in this status");

            serviceTask.SetOnHold();

            var repairItem = request.RepairItems.FirstOrDefault(r => r.Id == serviceTask.RepairItemId);

            if (repairItem == null)
                throw new NotFoundException("Repair item not found");

            if (repairItem.Status != RepairItemStatus.Draft &&
                repairItem.Status != RepairItemStatus.New &&
                repairItem.Status != RepairItemStatus.WaitingForParts &&
                repairItem.Status != RepairItemStatus.Completed &&
                repairItem.Status != RepairItemStatus.Cancelled &&
                repairItem.Status != RepairItemStatus.WaitingForPickUp
                )
                if (repairItem.ServiceTasks.All(s => s.Status == ServiceTaskStatus.OnHold || s.Status == ServiceTaskStatus.Cancelled))
                {
                    repairItem.SetOnHold();

                    if (repairItem.Status != RepairItemStatus.Draft &&
                        request.Status != RequestStatus.New &&
                        request.Status != RequestStatus.Completed &&
                        request.Status != RequestStatus.Cancelled &&
                        request.Status != RequestStatus.WaitingForPickUp &&
                        request.Status != RequestStatus.WaitingForParts &&
                        request.Status != RequestStatus.PickedUp
                        )
                        if (request.RepairItems.All(r => r.Status == RepairItemStatus.OnHold))
                            request.SetOnHold();
                }

            await context.SaveChangesAsync();

            return serviceTask;

            //var serviceTask = await context.ServiceTasks.FindAsync(id);

            //if (serviceTask == null)
            //    throw new NotFoundException("Service task not found");

            //if (serviceTask.Status == ServiceTaskStatus.Draft)
            //    throw new ConflictException("Not allowed in draft");

            //serviceTask.SetOnHold();

            //await context.SaveChangesAsync();

            //return serviceTask;
        }

        public async Task<ServiceTask> WaitForServiceTaskParts(int id)
        {
            var request = await context.Requests
                .Include(r => r.RepairItems)
                .ThenInclude(i => i.ServiceTasks)
                .FirstOrDefaultAsync(r => r.RepairItems.Any(i => i.ServiceTasks.Any(t => t.Id == id)));

            if (request == null)
                throw new NotFoundException("request not found");

            var serviceTask = request.RepairItems.SelectMany(s => s.ServiceTasks).FirstOrDefault(s => s.Id == id);

            if (serviceTask == null)
                throw new NotFoundException("Service task not found");

            if (serviceTask.Status == ServiceTaskStatus.Draft &&
                serviceTask.Status == ServiceTaskStatus.New &&
                serviceTask.Status == ServiceTaskStatus.OnHold &&
                serviceTask.Status == ServiceTaskStatus.Cancelled &&
                serviceTask.Status == ServiceTaskStatus.Completed
                )
                throw new ConflictException("Not allowed in this status");

            serviceTask.WaitForParts();

            var repairItem = request.RepairItems.FirstOrDefault(r => r.Id == serviceTask.RepairItemId);

            if (repairItem == null)
                throw new NotFoundException("Repair item not found");

            if (repairItem.Status != RepairItemStatus.Draft &&
                repairItem.Status != RepairItemStatus.New &&
                repairItem.Status != RepairItemStatus.OnHold &&
                repairItem.Status != RepairItemStatus.Completed &&
                repairItem.Status != RepairItemStatus.Cancelled &&
                repairItem.Status != RepairItemStatus.WaitingForPickUp
                )
                if (repairItem.ServiceTasks.All(s => s.Status == ServiceTaskStatus.WaitingForParts || s.Status == ServiceTaskStatus.Cancelled))
                {
                    repairItem.WaitForParts();

                    if (repairItem.Status != RepairItemStatus.Draft &&
                        request.Status != RequestStatus.New &&
                        request.Status != RequestStatus.OnHold &&
                        request.Status != RequestStatus.Completed &&
                        request.Status != RequestStatus.Cancelled &&
                        request.Status != RequestStatus.WaitingForPickUp &&
                        request.Status != RequestStatus.PickedUp
                        )
                        if (request.RepairItems.All(r => r.Status == RepairItemStatus.WaitingForParts))
                            request.WaitForParts();
                }

            await context.SaveChangesAsync();

            return serviceTask;

            //var serviceTask = await context.ServiceTasks.FindAsync(id);

            //if (serviceTask == null)
            //    throw new NotFoundException("Service task not found");

            //if (serviceTask.Status == ServiceTaskStatus.Draft)
            //    throw new ConflictException("Not allowed in draft");

            //serviceTask.Status = ServiceTaskStatus.WaitingForParts;

            //await context.SaveChangesAsync();

            //return serviceTask;
        }

        public async Task<ServiceTask> EditServiceTask(int id, EditServiceTaskDto dto)
        {
            var serviceTask = await context.ServiceTasks.FindAsync(id) ?? throw new NotFoundException("Service task not found"); //TODO статуси: в процесі і чернетка. Зробити шоб повтора не було

            var service = await context.Services.FindAsync(dto.ServiceId) ?? throw new NotFoundException("Service not found");

            if (serviceTask.Service.Status == ServiceStatus.Active && service.Status == ServiceStatus.Inactive)
                throw new ConflictException("Can`t add inactive service");

            serviceTask.UserId = dto.UserId;
            serviceTask.ServiceId = dto.ServiceId;

            await context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask?> GetServiceTaskById(int id)
        {
            return await context.ServiceTasks
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<ServiceTask>> GetAllServiceTasks()
        {
            return await context.ServiceTasks
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ServiceTask>> GetAllActiveServiceTasks(int userId, bool activeOnly)
        {
            var serviceTasks = context.ServiceTasks.Where(t => t.UserId == userId);

            if (activeOnly)
            {
                serviceTasks = serviceTasks.Where(t =>
                    t.Status != ServiceTaskStatus.Draft &&
                    t.Status != ServiceTaskStatus.Completed &&
                    t.Status != ServiceTaskStatus.Cancelled);
            }

            return await serviceTasks.ToListAsync();
        }
    }
}
