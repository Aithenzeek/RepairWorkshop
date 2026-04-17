using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;

        public CustomerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> CreateCustomer(CreateCustomerDto dto)
        {
            var phone = CheckPhone(dto.Phone);

            var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Phone == phone);

            if (existingCustomer != null)
                return existingCustomer;

            var customer = new Customer
            {
                Name = dto.Name,
                Phone = phone
            };

            await _context.Customers.AddAsync(customer);

            await _context.SaveChangesAsync();

            return customer;
        }

        public async void DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                throw new DirectoryNotFoundException("Customer not found");


            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();
        }

        public async Task<Customer> EditCustomer(string phone, CreateCustomerDto dto)
        {
            CheckPhone(dto.Phone);

            var customer = await GetCustomerByPhone(phone);

            if (customer == null)
                throw new DirectoryNotFoundException("Customer not found");

            customer.Name = dto.Name;
            customer.Phone = dto.Phone;

            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task<Customer> GetCustomerByPhone(string phone)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Phone == phone);
        }

        public async Task<List<Customer>> GetAllCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        public String CheckPhone(string phone)
        {
            var checkedPhone = new string(phone.Where(char.IsDigit).ToArray());

            if (phone.Length != 10)
                throw new BadRequestException("Phone not valid");

            return checkedPhone;
        }
    }
}
