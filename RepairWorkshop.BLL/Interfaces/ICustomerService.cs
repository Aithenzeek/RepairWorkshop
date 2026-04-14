using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface ICustomerService
    {
        public Task<Customer> CreateCustomer(CreateCustomerDto dto);
        public void DeleteCustomer(int id);
        public Task<Customer> EditCustomer(string phone, CreateCustomerDto dto);
        public Task<Customer> GetCustomerByPhone(string phone);
        public Task<List<Customer>> GetAllCustomers();
    }
}

