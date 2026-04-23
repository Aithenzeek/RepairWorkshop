using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Services
{
    public class UserService(AppDbContext context) : IUserService
    {
        public async Task<User> CreateUser(CreateUserDto dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Phone = dto.Phone,
                RoleId = dto.RoleId,
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return user;
        }

        public async Task DeleteUser(int id)
        {
            var user = await context.Users.FindAsync(id);

            if (user == null)
                throw new NotFoundException("User not found");

            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }

        public async Task<User> EditUser(int id, EditUserDto dto)
        {
            var user = await context.Users.FindAsync(id);

            if (user == null)
                throw new NotFoundException("User not found");

            user.Name = dto.Name;
            user.Phone = dto.Phone;
            user.RoleId = dto.RoleId;

            await context.SaveChangesAsync();

            return user;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await context.Users
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User?> GetUserById(int id)
        {
            return await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // треба тільки для назначення ролей
        public async Task<User?> GetUserByPhone(string phone)
        {
            return await context.Users
                .Include(u => u.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Phone == phone);
        }
    }
}
