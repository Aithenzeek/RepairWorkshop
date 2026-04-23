using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace RepairWorkshop.BLL.Services
{
    public class RolePermissionService(AppDbContext context) : IRolePermissionService
    {
        public readonly AppDbContext _context = context;

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

        public async Task DeleteRolePermission(int userRoleId, int permissionId)
        {
            var rolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(r => r.UserRoleId == userRoleId &&
                r.PermissionId == permissionId);

            if (rolePermission == null)
                throw new NotFoundException("role premission not found");

            _context.RolePermissions.Remove(rolePermission);
            await _context.SaveChangesAsync();
        }

        public async Task<RolePermission> EditRolePermission(int userRoleId, int permissionId, EditRolePermissionDto dto)
        {
            var rolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(r => r.UserRoleId == userRoleId &&
                r.PermissionId == permissionId);

            if (rolePermission == null)
                throw new NotFoundException("role premission not found");

            rolePermission.UserRoleId = dto.UserRoleId;
            rolePermission.PermissionId = dto.PermissionId;

            await _context.SaveChangesAsync();

            return rolePermission;
        }

        public async Task<RolePermission?> GetRolePermissionById(int userRoleId, int permissionId)
        {
            return await _context.RolePermissions
                .FirstOrDefaultAsync(r => r.UserRoleId == userRoleId &&
                r.PermissionId == permissionId);
        }

        public async Task<List<RolePermission>> GetAllRolePermissions()
        {
            return await _context.RolePermissions.ToListAsync();
        }
    }
}
