using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using RepairWorkShop.DAL.Enums;

namespace RepairWorkshop.BLL.Services
{
    public class ServiceService(AppDbContext context) : IServiceService
    {
        public async Task<Service> CreateService(CreateServiceDto dto)
        {
            var existingService = await context.Services.FirstOrDefaultAsync(s => s.Name == dto.Name);

            if (existingService != null && existingService.Name == dto.Name)
                throw new ConflictException("Service with this name exists");

            var service = new Service
            {
                Name = dto.Name,
                Price = dto.Price
            };

            await context.Services.AddAsync(service);
            await context.SaveChangesAsync();

            return service;
        }

        public async Task DeleteService(int id)
        {
            var service = await context.Services.FindAsync(id) ?? throw new NotFoundException("Service not found");
            
            service.Status = ServiceStatus.Inactive;

            //context.Services.Remove(service);
            await context.SaveChangesAsync();
        }

        public async Task<Service> EditService(int id, EditServiceDto dto)
        {
            var service = await context.Services.FindAsync(id) ?? throw new NotFoundException("Service not found");
            
            var existingService = await context.Services.FirstOrDefaultAsync(s => s.Name == dto.Name);

            if (existingService != null && (existingService.Name == dto.Name && service.Name != dto.Name))
                throw new ConflictException("Service with this name exists");

            service.Name = dto.Name;
            service.Price = dto.Price;

            await context.SaveChangesAsync();

            return service;
        }

        public async Task<List<Service>> GetAllServices()
        {
            return await context.Services
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Service?> GetServiceById(int id)
        {
            return await context.Services
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Service> ActivateService(int id)
        {
            var service = await context.Services.FindAsync(id) ?? throw new NotFoundException("Service not found");
            
            service.Status = ServiceStatus.Active;

            await context.SaveChangesAsync();

            return service;
        }

        public async Task<Service> InactivateService(int id)
        {
            var service = await context.Services.FindAsync(id) ?? throw new NotFoundException("Service not found");
            
            service.Status = ServiceStatus.Inactive;

            await context.SaveChangesAsync();

            return service;
        }

        public async Task<List<Service>> GetAllActiveServices()
        {
            return await context.Services
                .Where(s => s.Status == ServiceStatus.Active)
                .ToListAsync();
        }
    }
}
