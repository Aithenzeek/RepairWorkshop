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
            var userRole = await context.UserRoles.FindAsync(id);

            if (userRole == null)
                throw new NotFoundException("User role not found");

            context.UserRoles.Remove(userRole);
            await context.SaveChangesAsync();
        }

        public async Task<UserRole> EditUserRole(int id, EditUserRoleDto dto)
        {
            var userRole = await context.UserRoles.FindAsync(id);

            if (userRole == null)
                throw new NotFoundException("User role not found");

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
