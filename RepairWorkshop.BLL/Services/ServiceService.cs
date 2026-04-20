using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using RepairWorkShop.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RepairWorkshop.BLL.Services
{
    public class ServiceService : IServiceService
    {
        private readonly AppDbContext _context;

        public ServiceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Service> CreateService(CreateServiceDto dto)
        {
            var service = new Service
            {
                Name = dto.Name,
                Price = dto.Price
            };

            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();

            return service;
        }

        public async Task DeleteService(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
                throw new NotFoundException("Service not found");

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
        }

        public async Task<Service> EditService(int id, EditServiceDto dto)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
                throw new NotFoundException("Service not found");

            service.Name = dto.Name;
            service.Price = dto.Price;

            await _context.SaveChangesAsync();

            return service;
        }

        public async Task<List<Service>> GetAllServices()
        {
            return await _context.Services
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Service?> GetServiceById(int id)
        {
            return await _context.Services
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Service> ActivateService(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
                throw new NotFoundException("Service not found");

            service.Status = ServiceStatus.Active;

            await _context.SaveChangesAsync();

            return service;
        }

        public async Task<Service> InactivateService(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
                throw new NotFoundException("Service not found");

            service.Status = ServiceStatus.Inactive;

            await _context.SaveChangesAsync();

            return service;
        }

        public async Task<List<Service>> GetAllActiveServices()
        {
            return await _context.Services
                .Where(s => s.Status == ServiceStatus.Active)
                .ToListAsync();
        }
    }
}
