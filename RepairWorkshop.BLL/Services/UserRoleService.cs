using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly AppDbContext _context;

        public UserRoleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserRole> CreateUserRole(CreateUserRoleDto dto)
        {
            var userRole = new UserRole
            {
                Name = dto.Name,
            };

            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();

            return userRole;
        }

        public async void DeleteUserRole(int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);

            if (userRole == null)
                throw new NotFoundException("User role not found");

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task<UserRole> EditUserRole(int id, EditUserRoleDto dto)
        {
            var userRole = await _context.UserRoles.FindAsync(id);

            if (userRole == null)
                throw new NotFoundException("User role not found");

            userRole.Name = dto.Name;

            await _context.SaveChangesAsync();

            return userRole;
        }

        public async Task<List<UserRole>> GetAllUserRoles()
        {
            return await _context.UserRoles.ToListAsync();
        }

        public async Task<UserRole> GetUserRoleById(int id)
        {
            return await _context.UserRoles.FindAsync(id);
        }
    }
}
