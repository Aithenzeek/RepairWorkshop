using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface IServiceService
    {
        public Task<Service> CreateService(CreateServiceDto dto);
        public void DeleteService(int id);
        public Task<Service> EditService(int id, EditServiceDto dto);
        public Task<List<Service>> GetAllServices();
        public Task<Service> GetServiceById(int id);
    }
}
