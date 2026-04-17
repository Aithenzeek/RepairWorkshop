using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using RepairWorkShop.DAL.Enums;
using System.ComponentModel;

namespace RepairWorkshop.BLL.Services
{
    public class RepairItemService : IRepairItemService
    {
        private readonly AppDbContext _context;

        public RepairItemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RepairItem> CreateRepairItem(int requestId)
        {
            var request = await _context.Requests.FindAsync(requestId);

            if (request == null)
                throw new NotFoundException("request not found");

            if (request.Status != RequestStatus.Draft)
                throw new ConflictException("Cannot add to non-draft request");

            var repairItem = new RepairItem
            {
                CustomerRequestId = requestId,
                Status = RepairItemStatus.Draft,
            };

            await _context.RepairItems.AddAsync(repairItem);
            await _context.SaveChangesAsync();

            return repairItem;
        }

        public async void DeleteRepairItem(int id)
        {
            var repairItem = await _context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new DirectoryNotFoundException("Repair item not found");

            if (repairItem.Status != RepairItemStatus.Draft)
                throw new ConflictException("Only draft can be deleted");

            _context.RepairItems.Remove(repairItem);
            await _context.SaveChangesAsync();
        }

        public async Task<RepairItem> ApproveRepairItem(int id)
        {
            var repairItem = await _context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new DirectoryNotFoundException("Repair item not found");

            if (repairItem.Status == RepairItemStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            repairItem.Status = RepairItemStatus.OnHold;

            await _context.SaveChangesAsync();

            return repairItem;
        }

        public async Task<RepairItem> CompleteRepairItem(int id)
        {
            var repairItem = await _context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new DirectoryNotFoundException("Repair item not found");

            if (repairItem.Status == RepairItemStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            repairItem.Status = RepairItemStatus.Completed;

            await _context.SaveChangesAsync();

            return repairItem;
        }

        public async Task<RepairItem> CancelRepairItem(int id)
        {
            var repairItem = await _context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new DirectoryNotFoundException("Repair item not found");

            if (repairItem.Status == RepairItemStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            repairItem.Status = RepairItemStatus.Cancelled;

            // Переробити
            foreach(var task in repairItem.ServiceTasks)
                task.Status = ServiceTaskStatus.Cancelled;

            await _context.SaveChangesAsync();

            return repairItem;
        }

        public async Task<RepairItem> AllowPickUpRepairItem(int id)
        {
            var repairItem = await _context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new DirectoryNotFoundException("Repair item not found");

            if (repairItem.Status == RepairItemStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            repairItem.Status = RepairItemStatus.WaitingForPickUp;

            await _context.SaveChangesAsync();

            return repairItem;
        }

        public async Task<RepairItem> WaitForRepairItemParts(int id)
        {
            var repairItem = await _context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new DirectoryNotFoundException("Repair item not found");

            if (repairItem.Status == RepairItemStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            repairItem.Status = RepairItemStatus.WaitingForParts;

            await _context.SaveChangesAsync();

            return repairItem;
        }

        public async Task<RepairItem> SetOnHoldRepairItemWork(int id)
        {
            var repairItem = await _context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new DirectoryNotFoundException("Repair item not found");

            if (repairItem.Status == RepairItemStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            repairItem.Status = RepairItemStatus.OnHold;

            await _context.SaveChangesAsync();

            return repairItem;
        }

        public async Task<RepairItem> EditRepairItem(int id, EditRepairItemDto dto)
        {
            var serialNumber = await _context.RepairItems.AnyAsync(x => x.SerialNumber == dto.SerialNumber);

            if (serialNumber)
                throw new ConflictException("Item with such serial number exists");

            var repairItem = await _context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new DirectoryNotFoundException("Repair item not found");

            repairItem.Model = dto.Model;
            repairItem.SerialNumber = dto.SerialNumber;
            repairItem.ProblemDescription = dto.ProblemDescription;
            repairItem.Notes = dto.Notes;

            await _context.SaveChangesAsync();

            return repairItem;
        }

        public async Task<RepairItem> GetRepairItemById(int id)
        {
            return await _context.RepairItems.FindAsync(id);
        }

        public async Task<List<RepairItem>> GetAllRepairItems()
        {
            return await _context.RepairItems.ToListAsync();
        }

        public async Task<List<RepairItem>> GetAllActiveRepairItems(int workerId)
        {
            return await _context.ServiceTasks.Where(t => t.WorkerId == workerId).Select(t => t.RepairItem).Distinct().ToListAsync();
        }

        //public async Task<List<RepairItem>> GetAllActiveRepairItemsByItem(int id)
        //{
        //    return await _context.ServiceTasks.Where(t => t.RepairItemId == id && t.Status != ServiceTaskStatus.Draft).ToListAsync();
        //}
    }
}
