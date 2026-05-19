using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using RepairWorkShop.DAL.Enums;

namespace RepairWorkshop.BLL.Services
{
    public class UserService(AppDbContext context) : IUserService
    {
        public async Task<ResponseUserDto> CreateUser(CreateUserDto dto)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Phone == dto.Phone);
            var existingCustomer = await context.Customers.FirstOrDefaultAsync(c => c.Phone == dto.Phone);

            if (existingUser != null && dto.Phone != existingUser.Phone)
                throw new ConflictException("User with this number exists");

            if (existingCustomer != null &&  dto.Phone != existingCustomer.Phone)
                throw new ConflictException("Customer with this number exists");

            var existingRole = await context.UserRoles.FindAsync(dto.RoleId) ?? throw new NotFoundException("User role not found");
            
            var formattedPhone = CheckPhone(dto.Phone);

            var user = new User
            {
                Name = dto.Name,
                Phone = dto.Phone,
                RoleId = dto.RoleId,
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return await ReturnDto(user);
        }

        public async Task DeleteUser(int id)
        {
            var user = await context.Users.FindAsync(id) ?? throw new NotFoundException("User not found");

            var existingTasks = await context.ServiceTasks.AnyAsync(t => t.UserId == id &&
            t.Status != ServiceTaskStatus.Draft &&
            t.Status != ServiceTaskStatus.Completed &&
            t.Status != ServiceTaskStatus.Cancelled);

            if (existingTasks)
                throw new ConflictException("Cant delete user with active tasks");

            context.Users.Remove(user);

            await context.SaveChangesAsync();
        }

        public async Task<ResponseUserDto> EditUser(int id, EditUserDto dto)
        {
            var user = await context.Users.FindAsync(id) ?? throw new NotFoundException("User not found");

            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Phone == dto.Phone);
            var existingCustomer = await context.Customers.FirstOrDefaultAsync(c => c.Phone == dto.Phone);

            if (existingUser != null && (dto.Phone == existingUser.Phone && dto.Phone != existingUser.Phone))
                throw new ConflictException("User with this number exists");

            if (existingCustomer != null && dto.Phone != existingCustomer.Phone)
                throw new ConflictException("Customer with this number exists");

            var formattedPhone = CheckPhone(dto.Phone);

            var existingTasks = await context.ServiceTasks.AnyAsync(t => t.UserId == id &&
            t.Status != ServiceTaskStatus.Draft &&
            t.Status != ServiceTaskStatus.Completed &&
            t.Status != ServiceTaskStatus.Cancelled);

            if (existingTasks && user.RoleId != dto.RoleId)
                throw new ConflictException("Cant change role of user with active tasks");

            user.Name = dto.Name;
            user.Phone = dto.Phone;
            user.RoleId = dto.RoleId;

            await context.SaveChangesAsync();

            return await ReturnDto(user);
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .ToListAsync();
        }

        public async Task<List<User>> GetAllTechnicians()
        {
            return await context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Where(u => u.Role.Name == "Technician")
                .ToListAsync();
        }

        public async Task<User?> GetUserById(int id)
        {
            return await context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id) ?? throw new NotFoundException("User not found");
        }

        public async Task<User?> SearchUserByPhone(string phone)
        {
            return await context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Phone == phone);
        }

        // треба тільки для назначення ролей
        public async Task<User?> GetUserByPhone(string phone)
        {
            return await context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Phone == phone);
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

        public async Task<ResponseUserDto> ReturnDto(User user)
        {
            return new ResponseUserDto(
                user.Id,
                user.Name,
                user.Phone,
                user.RoleId
                );
        }
    }
}
