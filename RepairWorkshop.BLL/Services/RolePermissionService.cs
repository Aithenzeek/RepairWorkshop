using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepairWorkshop.BLL.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        public readonly AppDbContext _context;

        public RolePermissionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RolePermission> CreateRolePermission(CreateRolePermissionDto dto)
        {
            var rolePermission = new RolePermission
            {
                UserRoleId = dto.UserRoleId,
                PermissionId = dto.PermissionId,
            };

            await _context.RolePermissions.AddAsync(rolePermission);
            await _context.SaveChangesAsync();

            return rolePermission;
        }

        public async Task DeleteRolePermission(int id)
        {
            var rolePermission = await _context.RolePermissions.FindAsync(id);

            _context.RolePermissions.Remove(rolePermission);
            await _context.SaveChangesAsync();
        }

        // нема ід в нього, того не можна знайти одне.
        public async Task<RolePermission> EditRolePermission(int id, EditRolePermissionDto dto)
        {
            var rolePermission = await _context.RolePermissions.FindAsync(id);

            rolePermission.UserRoleId = dto.UserRoleId;
            rolePermission.PermissionId = dto.PermissionId;

            await _context.SaveChangesAsync();

            return rolePermission;
        }

        public async Task<RolePermission> GetRolePermissionById(int id)
        {
            return await _context.RolePermissions.FindAsync(id);
        }

        public async Task<List<RolePermission>> GetAllRolePermissions()
        {
            return await _context.RolePermissions.ToListAsync();
        }
    }
}
