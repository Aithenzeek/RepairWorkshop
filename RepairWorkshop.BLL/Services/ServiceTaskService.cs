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
                throw new NotFoundException("service not found");

            if (service.Status == ServiceStatus.Inactive)
                throw new ConflictException("Can`t add inactive service");

            var existingRepairItem = await context.RepairItems.FindAsync(dto.RepairItemId);

            if (existingRepairItem == null)
                throw new NotFoundException("repair item not found");

            var serviceTask = new ServiceTask
            {
                RepairItemId = dto.RepairItemId,
                UserId = dto.WorkerId,
                ServiceId = dto.ServiceId,
                Status = ServiceTaskStatus.Draft
            };

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

        public async Task<ServiceTask> CompleteServiceTask(int id)
        {
            var serviceTask = await context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new NotFoundException("Service task not found");

            if (serviceTask.Status != ServiceTaskStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            serviceTask.Status = ServiceTaskStatus.Completed;

            await context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask> CancelServiceTask(int id)
        {
            var serviceTask = await context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new NotFoundException("Service task not found");

            if (serviceTask.Status != ServiceTaskStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            serviceTask.Status = ServiceTaskStatus.Cancelled;

            await context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask> SetOnHoldServiceTask(int id)
        {
            var serviceTask = await context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new NotFoundException("Service task not found");

            if (serviceTask.Status != ServiceTaskStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            serviceTask.Status = ServiceTaskStatus.OnHold;

            await context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask> WaitForServiceTaskParts(int id)
        {
            var serviceTask = await context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new NotFoundException("Service task not found");

            if (serviceTask.Status != ServiceTaskStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            serviceTask.Status = ServiceTaskStatus.WaitingForParts;

            await context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask> EditServiceTask(int id, EditServiceTaskDto dto)
        {
            var serviceTask = await context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new NotFoundException("Service task not found"); //TODO статуси: в процесі і чернетка

            var service = await context.Services.FindAsync(dto.ServiceId);

            if (service == null)
                throw new NotFoundException("service not found");

            if (serviceTask.Service.Status == ServiceStatus.Active && service.Status == ServiceStatus.Inactive)
                throw new ConflictException("Can`t add inactive service");

            serviceTask.UserId = dto.WorkerId;
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
