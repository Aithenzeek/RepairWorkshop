using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Services
{
    public class RolePermissionService(AppDbContext context) : IRolePermissionService
    {
        public async Task<RolePermission> CreateRolePermission(CreateRolePermissionDto dto)
        {
            var existingRolePermission = await context.RolePermissions.FirstOrDefaultAsync(r =>
            r.UserRoleId == dto.UserRoleId &&
            r.PermissionId == dto.PermissionId);

            if (await context.Permissions.FindAsync(dto.PermissionId) == null)
                throw new NotFoundException("Permission not found");

            if (existingRolePermission != null)
                throw new ConflictException("This role permission exists");

            var rolePermission = new RolePermission
            {
                UserRoleId = dto.UserRoleId,
                PermissionId = dto.PermissionId,
            };

            await context.RolePermissions.AddAsync(rolePermission);
            await context.SaveChangesAsync();

            return rolePermission;
        }

        public async Task DeleteRolePermission(int userRoleId, int permissionId)
        {
            var rolePermission = await context.RolePermissions
                .FirstOrDefaultAsync(r => r.UserRoleId == userRoleId &&
                r.PermissionId == permissionId);

            if (rolePermission == null)
                throw new NotFoundException("role premission not found");

            context.RolePermissions.Remove(rolePermission);
            await context.SaveChangesAsync();
        }

        public async Task<RolePermission> EditRolePermission(int userRoleId, int permissionId, EditRolePermissionDto dto) // TODO: може забрати взагалі, бо легше буде нове створити чим редагувати старе
        {
            var rolePermission = await context.RolePermissions
                .FirstOrDefaultAsync(r => r.UserRoleId == userRoleId &&
                r.PermissionId == permissionId);

            if (rolePermission == null)
                throw new NotFoundException("role premission not found");

            var existingRolePermission = await context.RolePermissions.FirstOrDefaultAsync(r =>
            r.UserRoleId == dto.UserRoleId &&
            r.PermissionId == dto.PermissionId);

            if (existingRolePermission != null)
                throw new ConflictException("Role permision with this user role and permission exists");

            rolePermission.UserRoleId = dto.UserRoleId;
            rolePermission.PermissionId = dto.PermissionId;

            await context.SaveChangesAsync();

            return rolePermission;
        }

        public async Task<RolePermission?> GetRolePermissionById(int userRoleId, int permissionId)
        {
            return await context.RolePermissions
                .FirstOrDefaultAsync(r => r.UserRoleId == userRoleId &&
                r.PermissionId == permissionId);
        }

        public async Task<List<RolePermission>> GetAllRolePermissions()
        {
            return await context.RolePermissions.ToListAsync();
        }
    }
}
