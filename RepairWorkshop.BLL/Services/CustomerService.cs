using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using RepairWorkShop.DAL.Enums;

namespace RepairWorkshop.BLL.Services
{
    public class CustomerService(AppDbContext context) : ICustomerService
    {
        public async Task<Customer> CreateCustomer(CreateCustomerDto dto)
        {
            var phone = CheckPhone(dto.Phone);

            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Phone == dto.Phone);
            var existingCustomer = await context.Customers.FirstOrDefaultAsync(c => c.Phone == phone);

            if (existingUser != null)
                throw new ConflictException("User with this phone exists");

            if (existingCustomer != null)
                return existingCustomer;

            var customer = new Customer
            {
                Name = dto.Name,
                Phone = phone
            };

            await context.Customers.AddAsync(customer);

            await context.SaveChangesAsync();

            return customer;
        }

        public async Task DeleteCustomer(int id)
        {
            var customer = await context.Customers.FindAsync(id) ?? throw new NotFoundException("Customer not found");

            var requests = await context.Requests
                .AnyAsync(r => r.CustomerId == id &&
                    r.Status != RequestStatus.Draft &&
                    r.Status != RequestStatus.Completed &&
                    r.Status != RequestStatus.Completed &&
                    r.Status != RequestStatus.PickedUp);

            if (requests == true)
                throw new ConflictException("Cant delete customer with active requests");

            context.Customers.Remove(customer);

            await context.SaveChangesAsync();
        }

        public async Task<Customer> EditCustomer(int id, EditCustomerDto dto)
        {
            var formattedPhone = CheckPhone(dto.Phone);

            var customer = await context.Customers.FindAsync(id);

            if (customer == null)
                throw new NotFoundException("Customer not found");

            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Phone == dto.Phone);
            var existingCustomer = await context.Customers.FirstOrDefaultAsync(c => c.Phone == formattedPhone);

            if (existingUser != null)
                throw new ConflictException("User with this phone exists");

            if (existingCustomer != null && customer.Phone != dto.Phone)
                throw new ConflictException("Customer with this number exists");

            customer.Name = dto.Name;
            customer.Phone = formattedPhone;

            await context.SaveChangesAsync();

            return customer;
        }

        public async Task<Customer?> GetCustomerByPhone(string phone)
        {
            return await context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Phone == CheckPhone(phone));
        }

        public async Task<List<Customer>> GetAllCustomers()
        {
            return await context.Customers
                .AsNoTracking()
                .ToListAsync();
        }

        public static string CheckPhone(string phone)
        {
            var checkedPhone = new string(phone.Where(char.IsDigit).ToArray());

            if (checkedPhone.Length == 12 && checkedPhone.StartsWith("380"))
                return checkedPhone;

            if (checkedPhone.Length == 10)
                return "38" + checkedPhone;

            throw new BadRequestException("Phone not valid");
        }
    }
}
