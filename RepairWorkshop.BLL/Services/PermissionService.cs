using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Services
{
    public class PermissionService(AppDbContext context) : IPermissionService
    {
        public async Task<Permission> CreatePermission(CreatePermissionDto dto)
        {
            var existingPermission = await context.Permissions.FirstOrDefaultAsync(p => p.Name == dto.Name);

            if (existingPermission.Name == dto.Name || existingPermission.Code == dto.Code)
                throw new ConflictException("Permission with this parameters exists");

            var permission = new Permission
            {
                Name = dto.Name,
                Code = dto.Code
            };

            await context.AddAsync(permission);
            await context.SaveChangesAsync();

            return permission;
        }

        public async Task DeletePermission(int id)
        {
            var permission = await context.Permissions.FindAsync(id);

            if (permission == null)
                throw new NotFoundException("premission not found");

            context.Permissions.Remove(permission);
            await context.SaveChangesAsync();
        }

        public async Task<Permission> EditPermission(int id, EditPermissionDto dto)
        {
            var permission = await context.Permissions.FindAsync(id);

            if (permission == null)
                throw new NotFoundException("premission not found");

            var existingPermission = await context.Permissions.FirstOrDefaultAsync(p => p.Name == dto.Name);

            if (existingPermission.Name == dto.Name || existingPermission.Code == dto.Code)
                throw new ConflictException("Permission with this parameters exists");

            permission.Name = dto.Name;
            permission.Code = dto.Code;

            await context.SaveChangesAsync();

            return permission;
        }

        public async Task<Permission?> GetPermissionById(int id)
        {
            return await context.Permissions.FindAsync(id);
        }

        public async Task<List<Permission>> GetAllPermissions()
        {
            return await context.Permissions.ToListAsync();
        }
    }
}
