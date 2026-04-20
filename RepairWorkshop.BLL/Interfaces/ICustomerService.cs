using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface ICustomerService
    {
        public Task<Customer> CreateCustomer(CreateCustomerDto dto);
        public void DeleteCustomer(int id);
        public Task<Customer> EditCustomer(int id, CreateCustomerDto dto);
        public Task<Customer?> GetCustomerByPhone(string phone);
        public Task<List<Customer>> GetAllCustomers();
    }
}

