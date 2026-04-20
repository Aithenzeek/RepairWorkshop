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

        public async Task DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                throw new NotFoundException("Customer not found");


            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();
        }

        public async Task<Customer> EditCustomer(int id, EditCustomerDto dto)
        {
            var formattedPhone = CheckPhone(dto.Phone);

            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                throw new NotFoundException("Customer not found");

            customer.Name = dto.Name;
            customer.Phone = formattedPhone;

            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task<Customer?> GetCustomerByPhone(string phone)
        {
            return await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Phone == phone);
        }

        public async Task<List<Customer>> GetAllCustomers()
        {
            return await _context.Customers
                .AsNoTracking()
                .ToListAsync();
        }

        public static string CheckPhone(string phone)
        {
            var checkedPhone = new string(phone.Where(char.IsDigit).ToArray());

            //if (checkedPhone.Length != 10)
            //    throw new BadRequestException("Phone not valid");

            if (checkedPhone.Length == 12 && checkedPhone.StartsWith("380"))
                return checkedPhone;

            if (checkedPhone.Length == 10)
                return "380" + checkedPhone;

            throw new BadRequestException("Phone not valid");
        }
    }
}
