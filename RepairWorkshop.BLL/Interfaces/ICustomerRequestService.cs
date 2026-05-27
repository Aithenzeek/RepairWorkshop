using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface ICustomerRequestService
    {
        public Task<ResponseCustomerRequestDto> CreateRequest(CreateCustomerRequestDto dto, int managerId);
        public Task DeleteRequest(int id);
        public Task<ResponseCustomerRequestDto> CancelRequest(int id, CancelCustomerRequestDto dto);
        public Task<ResponseCustomerRequestDto> CompleteRequest(int id);
        public Task<ResponseCustomerRequestDto> AllowPickUp(int id);
        public Task<CustomerRequest?> GetRequestById(int id);
        public Task<List<CustomerRequest>> GetAllRequests();
        public Task<ResponseCustomerRequestDto> PickUp(int id);
        public Task<ResponseCustomerRequestDto> EditRequest(int id, EditCustomerRequestDto dto);
        public Task<ResponseCustomerRequestDto> StartRequest(int id);
        public Task<PagedResponse<ResponseCustomerRequestDto>> GetPaged(RequestFilterDto filter);
    }
}
