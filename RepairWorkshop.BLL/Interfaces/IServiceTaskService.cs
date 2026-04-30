using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface IServiceTaskService
    {
        public Task<ServiceTask> CreateServiceTask(CreateServiceTaskDto dto);
        public Task DeleteServiceTask(int id);
        public Task<ServiceTask> StartServiceTask(int id);
        public Task<ServiceTask> CompleteServiceTask(CompleteServiceTaskDto dto);
        public Task<ServiceTask> CancelServiceTask(int id);
        public Task<ServiceTask> SetOnHoldServiceTask(int id);
        public Task<ServiceTask> WaitForServiceTaskParts(int id);
        public Task<ServiceTask> EditServiceTask(int id, EditServiceTaskDto dto);
        public Task<ServiceTask?> GetServiceTaskById(int id);
        public Task<List<ServiceTask>> GetAllServiceTasks();
        public Task<List<ServiceTask>> GetAllActiveServiceTasks(int userId, bool activeOnly);
    }
}
