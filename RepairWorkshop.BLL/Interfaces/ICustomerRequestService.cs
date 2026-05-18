using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface ICustomerRequestService
    {
        public Task<ResponseCustomerRequestDto> CreateRequest(/*int managerId, */CreateCustomerRequestDto dto, int managerId);
        public Task DeleteRequest(int id);
        public Task<ResponseCustomerRequestDto> CancelRequest(int id);
        public Task<ResponseCustomerRequestDto> CompleteRequest(int id);
        //public Task<CustomerRequest> ApproveRequest(int id);
        public Task<ResponseCustomerRequestDto> AllowPickUp(int id);
        public Task<ResponseCustomerRequestDto?> GetRequestById(int id);
        public Task<List<CustomerRequest>> GetAllRequests();
        public Task<ResponseCustomerRequestDto> EditRequest(int id, EditCustomerRequestDto dto);
        public Task<ResponseCustomerRequestDto> StartRequest(int id);
    }
}
