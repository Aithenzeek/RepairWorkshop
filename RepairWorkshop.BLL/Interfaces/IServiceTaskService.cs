using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface IServiceTaskService
    {
        public Task<ResponseServiceTaskDto> CreateServiceTask(CreateServiceTaskDto dto);
        public Task DeleteServiceTask(int id);
        public Task<ResponseServiceTaskDto> StartServiceTask(int id, int technicianId);
        public Task<ResponseServiceTaskDto> CompleteServiceTask(CompleteServiceTaskDto dto, int technicianId);
        public Task<ResponseServiceTaskDto> CancelServiceTask(int id, int technicianId, CancelServiceTaskDto dto);
        public Task<ResponseServiceTaskDto> SetOnHoldServiceTask(int id, int technicianId);
        public Task<ResponseServiceTaskDto> WaitForServiceTaskParts(int id, int technicianId);
        public Task<ResponseServiceTaskDto> EditServiceTask(int id, EditServiceTaskDto dto);
        public Task<ServiceTask?> GetServiceTaskById(int id);
        public Task<List<ServiceTask>> GetAllServiceTasks();
        public Task<List<ServiceTask>> GetAllActiveServiceTasks(int userId, bool activeOnly);
        public Task<PagedResponse<ResponseServiceTaskDto>> GetPaged(int page = 1, int pageSize = 10);
        public Task<PagedResponse<ResponseServiceTaskDto>> GetActivePaged(int userId, bool activeOnly, int page = 1, int pageSize = 10);
    }
}
