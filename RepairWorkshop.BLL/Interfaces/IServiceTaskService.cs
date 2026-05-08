using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface IServiceTaskService
    {
        public Task<ResponseServiceTaskDto> CreateServiceTask(CreateServiceTaskDto dto);
        public Task DeleteServiceTask(int id);
        public Task<ResponseServiceTaskDto> StartServiceTask(int id);
        public Task<ResponseServiceTaskDto> CompleteServiceTask(CompleteServiceTaskDto dto);
        public Task<ResponseServiceTaskDto> CancelServiceTask(int id);
        public Task<ResponseServiceTaskDto> SetOnHoldServiceTask(int id);
        public Task<ResponseServiceTaskDto> WaitForServiceTaskParts(int id);
        public Task<ResponseServiceTaskDto> EditServiceTask(int id, EditServiceTaskDto dto);
        public Task<ServiceTask?> GetServiceTaskById(int id);
        public Task<List<ServiceTask>> GetAllServiceTasks();
        public Task<List<ServiceTask>> GetAllActiveServiceTasks(int userId, bool activeOnly);
    }
}
