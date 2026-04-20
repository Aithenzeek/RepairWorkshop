using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface ICustomerService
    {
        public Task<Customer> CreateCustomer(CreateCustomerDto dto);
        public Task DeleteCustomer(int id);
        public Task<Customer> EditCustomer(int id, EditCustomerDto dto);
        public Task<Customer?> GetCustomerByPhone(string phone);
        public Task<List<Customer>> GetAllCustomers();
    }
}

