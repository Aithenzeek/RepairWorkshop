using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface IServiceService
    {
        public Task<ResponseServiceDto> CreateService(CreateServiceDto dto);
        public Task DeleteService(int id);
        public Task<ResponseServiceDto> EditService(int id, EditServiceDto dto);
        public Task<List<Service>> GetAllServices();
        public Task<Service?> GetServiceById(int id);
        public Task<ResponseServiceDto> ActivateService(int id);
        public Task<ResponseServiceDto> InactivateService(int id);
    }
}
