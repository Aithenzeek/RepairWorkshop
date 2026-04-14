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

        public ServiceTask CreateServiceTask(int repairItemId, CreateServiceTaskDto dto)
        {
            var serviceTask = new ServiceTask();

            serviceTask.RepairItemId = repairItemId;
            serviceTask.WorkerId = dto.WorkerId;
            serviceTask.ServiceId = dto.ServiceId;
            serviceTask.Status = ServiceTaskStatus.New;

            _context.ServiceTasks.Add(serviceTask);
            _context.SaveChanges();

            return serviceTask;
        }

        public ServiceTask DeleteServiceTask(int id)
        {
            var serviceTask = _context.ServiceTasks.Find(id);

            if (serviceTask == null)
                throw new Exception("Service task not found");

            _context.ServiceTasks.Remove(serviceTask);
            _context.SaveChanges();

            return serviceTask;
        }

        public ServiceTask CompleteServiceTask(int id)
        {
            var serviceTask = _context.ServiceTasks.Find(id);

            if (serviceTask == null)
                throw new Exception("Service task not found");

            serviceTask.Status = ServiceTaskStatus.Completed;

            _context.SaveChanges();

            return serviceTask;
        }

        public ServiceTask CancelServiceTask(int id)
        {
            var serviceTask = _context.ServiceTasks.Find(id);

            if (serviceTask == null)
                throw new Exception("Service task not found");

            serviceTask.Status = ServiceTaskStatus.Cancelled;

            _context.SaveChanges();

            return serviceTask;
        }

        public ServiceTask SetOnHoldServiceTask(int id)
        {
            var serviceTask = _context.ServiceTasks.Find(id);

            if (serviceTask == null)
                throw new Exception("Service task not found");

            serviceTask.Status = ServiceTaskStatus.OnHold;

            _context.SaveChanges();

            return serviceTask;
        }

        public ServiceTask WaitForServiceTaskParts(int id)
        {
            var serviceTask = _context.ServiceTasks.Find(id);

            if (serviceTask == null)
                throw new Exception("Service task not found");

            serviceTask.Status = ServiceTaskStatus.WaitingForParts;

            _context.SaveChanges();

            return serviceTask;
        }

        public ServiceTask EditServiceTask(int id, CreateServiceTaskDto dto)
        {
            var serviceTask = _context.ServiceTasks.Find(id);

            if (serviceTask == null)
                throw new Exception("Service task not found");

            serviceTask.WorkerId = dto.WorkerId;
            serviceTask.ServiceId = dto.ServiceId;

            _context.SaveChanges();

            return serviceTask;
        }
    }
}
