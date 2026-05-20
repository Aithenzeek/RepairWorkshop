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
        public async Task<ResponsePermissionDto> CreatePermission(CreatePermissionDto dto)
        {
            var existingPermission = await context.Permissions.FirstOrDefaultAsync(p => p.Name == dto.Name);

            if (existingPermission != null && (existingPermission.Name == dto.Name || existingPermission.Code == dto.Code))
                throw new ConflictException("Permission with this parameters exists");

            var permission = new Permission
            {
                Name = dto.Name,
                Code = dto.Code
            };

            await context.AddAsync(permission);
            await context.SaveChangesAsync();

            return await ReturnDto(permission);
        }

        public async Task DeletePermission(int id)
        {
            var permission = await context.Permissions.FindAsync(id) ?? throw new NotFoundException("premission not found");
            
            context.Permissions.Remove(permission);
            await context.SaveChangesAsync();
        }

        public async Task<ResponsePermissionDto> EditPermission(int id, EditPermissionDto dto)
        {
            var permission = await context.Permissions.FindAsync(id) ?? throw new NotFoundException("premission not found");
            
            var existingPermission = await context.Permissions.FirstOrDefaultAsync(p => p.Name == dto.Name);

            if (existingPermission != null && (existingPermission.Name == dto.Name || existingPermission.Code == dto.Code) && existingPermission.Id != permission.Id)
                throw new ConflictException("Permission with this parameters exists");

            permission.Name = dto.Name;
            permission.Code = dto.Code;

            await context.SaveChangesAsync();

            return await ReturnDto(permission);
        }

        public async Task<Permission?> GetPermissionById(int id)
        {
            return await context.Permissions.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id) ?? throw new NotFoundException("Permission not found");
        }

        public async Task<List<Permission>> GetAllPermissions()
        {
            return await context.Permissions.ToListAsync();
        }

        public async Task<ResponsePermissionDto> ReturnDto(Permission permission)
        {
            return new ResponsePermissionDto(
                permission.Id,
                permission.Name,
                permission.Code
                );
        }

        public async Task<PagedResponse<ResponsePermissionDto>> GetPaged(int page = 1, int pageSize = 10)
        {
            var query = context.Permissions;

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ResponsePermissionDto(
                    x.Id,
                    x.Name,
                    x.Code
                ))
                .ToListAsync();

            return new PagedResponse<ResponsePermissionDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
