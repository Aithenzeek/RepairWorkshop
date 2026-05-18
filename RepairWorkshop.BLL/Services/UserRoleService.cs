using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Services
{
    public class UserRoleService(AppDbContext context) : IUserRoleService
    {
        public async Task<UserRole> CreateUserRole(CreateUserRoleDto dto)
        {
            var existingUserRole = await context.UserRoles.FirstOrDefaultAsync(u => u.Name == dto.Name);

            // можна так
            //if(await context.UserRoles.AnyAsync(u => u.Name == dto.Name) != null)

            if (existingUserRole != null)
                throw new ConflictException("User role with this name exists");

            var userRole = new UserRole
            {
                Name = dto.Name,
            };

            await context.UserRoles.AddAsync(userRole);
            await context.SaveChangesAsync();

            return userRole;
        }

        public async Task DeleteUserRole(int id)
        {
            var userRole = await context.UserRoles.FindAsync(id) ?? throw new NotFoundException("User role not found");
            
            if (await context.Users.AnyAsync(u => u.Role.Id == id))
                throw new ConflictException("Cant delete with existing users");

            context.UserRoles.Remove(userRole);
            await context.SaveChangesAsync();
        }

        public async Task<UserRole> EditUserRole(int id, EditUserRoleDto dto) // TODO:вирішити як це зробити, бо назву можна змінити, але токени збережуться якими були
        {
            var userRole = await context.UserRoles.FindAsync(id) ?? throw new NotFoundException("User role not found");
            
            userRole.Name = dto.Name;

            await context.SaveChangesAsync();

            return userRole;
        }

        public async Task<List<UserRole>> GetAllUserRoles()
        {
            return await context.UserRoles
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserRole?> GetUserRoleById(int id)
        {
            return await context.UserRoles
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
