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
        public async Task<ResponseRolePermission> CreateRolePermission(CreateRolePermissionDto dto)
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

            return await ReturnDto(rolePermission);
        }

        public async Task DeleteRolePermission(int userRoleId, int permissionId)
        {
            var rolePermission = await context.RolePermissions
                .FirstOrDefaultAsync(r => r.UserRoleId == userRoleId &&
                r.PermissionId == permissionId) ?? throw new NotFoundException("Role premission not found");
            
            context.RolePermissions.Remove(rolePermission);
            await context.SaveChangesAsync();
        }

        public async Task<ResponseRolePermission> EditRolePermission(int userRoleId, int permissionId, EditRolePermissionDto dto) // не використовується
        {
            var rolePermission = await context.RolePermissions
                .FirstOrDefaultAsync(r => r.UserRoleId == userRoleId &&
                r.PermissionId == permissionId) ?? throw new NotFoundException("Role premission not found");
            
            var existingRolePermission = await context.RolePermissions.FirstOrDefaultAsync(r =>
            r.UserRoleId == dto.UserRoleId &&
            r.PermissionId == dto.PermissionId);

            if (existingRolePermission != null)
                throw new ConflictException("Role permision with this user role and permission exists");

            rolePermission.UserRoleId = dto.UserRoleId;
            rolePermission.PermissionId = dto.PermissionId;

            await context.SaveChangesAsync();

            return await ReturnDto(rolePermission);
        }

        public async Task<RolePermission?> GetRolePermissionById(int userRoleId, int permissionId) // не використовується
        {
            return await context.RolePermissions
                .FirstOrDefaultAsync(r => r.UserRoleId == userRoleId &&
                r.PermissionId == permissionId) ?? throw new NotFoundException("Role permission not found");
        }

        public async Task<List<RolePermission>> GetAllRolePermissions()
        {
            return await context.RolePermissions.ToListAsync();
        }

        public async Task<ResponseRolePermission> ReturnDto(RolePermission rolePermission)
        {
            return new ResponseRolePermission(
                rolePermission.UserRoleId,
                rolePermission.PermissionId
                );
        }

        public async Task<PagedResponse<ResponseRolePermission>> GetPaged(int page = 1, int pageSize = 10)
        {
            var query = context.RolePermissions;

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ResponseRolePermission(
                    x.UserRoleId,
                    x.PermissionId
                ))
                .ToListAsync();

            return new PagedResponse<ResponseRolePermission>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
