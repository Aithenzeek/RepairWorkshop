using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface ICustomerService
    {
        public Task<ResponseCustomerDto> CreateCustomer(CreateCustomerDto dto);
        public Task DeleteCustomer(int id);
        public Task<ResponseCustomerDto> EditCustomer(int id, EditCustomerDto dto);
        public Task<Customer?> GetCustomerByPhone(string phone);
        public Task<List<Customer>> GetAllCustomers();
        public Task<PagedResponse<ResponseCustomerDto>> GetPaged(int page = 1, int pageSize = 10);
    }
}

