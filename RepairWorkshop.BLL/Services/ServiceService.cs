using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using System;
using System.Collections.Generic;
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

        public async void DeleteService(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
                throw new NotFoundException("Service not found");

            _context.Services.Remove(service);
            _context.SaveChanges();
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
            return await _context.Services.ToListAsync();
        }

        public async Task<Service> GetServiceById(int id)
        {
            return await _context.Services.FindAsync(id);
        }
    }
}
