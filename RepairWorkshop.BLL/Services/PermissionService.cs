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
    public class PermissionService : IPermissionService
    {
        private readonly AppDbContext _context;

        public PermissionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Permission> CreatePermission(CreatePermissionDto dto)
        {
            var permission = new Permission
            {
                Name = dto.Name,
                Code = dto.Code
            };

            await _context.AddAsync(permission);
            await _context.SaveChangesAsync();

            return permission;
        }

        public async Task DeletePermission(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();
        }

        public async Task<Permission> EditPermission(int id, EditPermissionDto dto)
        {
            var permission = await _context.Permissions.FindAsync(id);

            permission.Name = dto.Name;
            permission.Code = dto.Code;

            await _context.SaveChangesAsync();

            return permission;
        }

        public async Task<Permission> GetPermissionById(int id)
        {
            return await _context.Permissions.FindAsync(id);
        }

        public async Task<List<Permission>> GetAllPermissions()
        {
            return await _context.Permissions.ToListAsync();
        }
    }
}
