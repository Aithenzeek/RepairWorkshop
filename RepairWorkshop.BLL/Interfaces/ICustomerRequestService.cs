using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface ICustomerRequestService
    {
        public Task<CustomerRequest> CreateRequest(int managerId);
        public void DeleteRequest(int id);
        public Task<CustomerRequest> CancelRequest(int id);
        public Task<CustomerRequest> CompleteRequest(int id);
        public Task<CustomerRequest> ApproveRequest(int id);
        public Task<CustomerRequest> AllowPickUp(int id);
        public Task<CustomerRequest?> GetRequestById(int id);
        public Task<List<CustomerRequest>> GetAllRequests();
        public Task<CustomerRequest> EditRequest(int id, EditCustomerRequestDto dto);
    }
}
