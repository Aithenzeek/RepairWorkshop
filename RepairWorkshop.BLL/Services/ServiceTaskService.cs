using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using RepairWorkShop.DAL.Enums;

namespace RepairWorkshop.BLL.Services
{
    public class ServiceTaskService : IServiceTaskService
    {
        private AppDbContext _context;

        public ServiceTaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceTask> CreateServiceTask(int repairItemId, CreateServiceTaskDto dto)
        {
            var existingServiceTask = await _context.ServiceTasks.FindAsync(dto.ServiceId);

            if(existingServiceTask != null && existingServiceTask.RepairItemId == repairItemId)
                throw new ConflictException("Task with same service exists");

            var service = await _context.Services.FindAsync(dto.ServiceId);

            if(service.Status == ServiceStatus.Inactive)
                throw new ConflictException("Can`t add inactive service");

            var serviceTask = new ServiceTask
            {
                RepairItemId = repairItemId,
                UserId = dto.WorkerId,
                ServiceId = dto.ServiceId,
                Status = ServiceTaskStatus.Draft
            };

            await _context.ServiceTasks.AddAsync(serviceTask);
            await _context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task DeleteServiceTask(int id)
        {
            var serviceTask = await _context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new NotFoundException("Service task not found");

            if (serviceTask.Status != ServiceTaskStatus.Draft)
                throw new ConflictException("Only draft can be deleted");

            _context.ServiceTasks.Remove(serviceTask);
            await _context.SaveChangesAsync();
        }

        public async Task<ServiceTask> CompleteServiceTask(int id)
        {
            var serviceTask = await _context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new NotFoundException("Service task not found");

            if (serviceTask.Status != ServiceTaskStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            serviceTask.Status = ServiceTaskStatus.Completed;

            await _context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask> CancelServiceTask(int id)
        {
            var serviceTask = await _context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new NotFoundException("Service task not found");

            if (serviceTask.Status != ServiceTaskStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            serviceTask.Status = ServiceTaskStatus.Cancelled;

            await _context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask> SetOnHoldServiceTask(int id)
        {
            var serviceTask = await _context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new NotFoundException("Service task not found");

            if (serviceTask.Status != ServiceTaskStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            serviceTask.Status = ServiceTaskStatus.OnHold;

            await _context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask> WaitForServiceTaskParts(int id)
        {
            var serviceTask = await _context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new NotFoundException("Service task not found");

            if (serviceTask.Status != ServiceTaskStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            serviceTask.Status = ServiceTaskStatus.WaitingForParts;

            await _context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask> EditServiceTask(int id, EditServiceTaskDto dto)
        {
            var serviceTask = await _context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new NotFoundException("Service task not found"); //TODO статуси: в процесі і чернетка

            var service = await _context.Services.FindAsync(dto.ServiceId);

            if (serviceTask.Service.Status == ServiceStatus.Active && service.Status == ServiceStatus.Inactive)
                throw new ConflictException("Can`t add inactive service");

            serviceTask.UserId = dto.WorkerId;
            serviceTask.ServiceId = dto.ServiceId;

            await _context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask?> GetServiceTaskById(int id)
        {
            return await _context.ServiceTasks
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<ServiceTask>> GetAllServiceTasks()
        {
            return await _context.ServiceTasks
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
