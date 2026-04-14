using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using RepairWorkShop.DAL.Enums;

namespace RepairWorkshop.BLL.Services
{
    public class ServiceTaskService
    {
        private AppDbContext _context;

        public ServiceTaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceTask> CreateServiceTask(int repairItemId, CreateServiceTaskDto dto)
        {
            var serviceTask = new ServiceTask();

            serviceTask.RepairItemId = repairItemId;
            serviceTask.WorkerId = dto.WorkerId;
            serviceTask.ServiceId = dto.ServiceId;
            serviceTask.Status = ServiceTaskStatus.New;

            await _context.ServiceTasks.AddAsync(serviceTask);
            await _context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask> DeleteServiceTask(int id)
        {
            var serviceTask = await _context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new DirectoryNotFoundException("Service task not found");

            _context.ServiceTasks.Remove(serviceTask);
            await _context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask> CompleteServiceTask(int id)
        {
            var serviceTask = await _context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new DirectoryNotFoundException("Service task not found");

            serviceTask.Status = ServiceTaskStatus.Completed;

            await _context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask> CancelServiceTask(int id)
        {
            var serviceTask = await _context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new DirectoryNotFoundException("Service task not found");

            serviceTask.Status = ServiceTaskStatus.Cancelled;

            await _context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask> SetOnHoldServiceTask(int id)
        {
            var serviceTask = await _context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new DirectoryNotFoundException("Service task not found");

            serviceTask.Status = ServiceTaskStatus.OnHold;

            await _context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask> WaitForServiceTaskParts(int id)
        {
            var serviceTask = await _context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new DirectoryNotFoundException("Service task not found");

            serviceTask.Status = ServiceTaskStatus.WaitingForParts;

            await _context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask> EditServiceTask(int id, CreateServiceTaskDto dto)
        {
            var serviceTask = await _context.ServiceTasks.FindAsync(id);

            if (serviceTask == null)
                throw new DirectoryNotFoundException("Service task found");

            serviceTask.WorkerId = dto.WorkerId;
            serviceTask.ServiceId = dto.ServiceId;

            await _context.SaveChangesAsync();

            return serviceTask;
        }

        public async Task<ServiceTask> GetServiceTaskById(int id)
        {
            return await _context.ServiceTasks.FindAsync(id);
        }

        public async Task<List<ServiceTask>> GetAllServiceTasks()
        {
            return await _context.ServiceTasks.ToListAsync();
        }
    }
}
