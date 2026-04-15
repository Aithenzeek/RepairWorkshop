using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using RepairWorkShop.DAL.Enums;
using System.ComponentModel;

namespace RepairWorkshop.BLL.Services
{
    public class RepairItemService
    {
        private readonly AppDbContext _context;

        public RepairItemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RepairItem> CreateRepairItem(int requestId)
        {
            var repairItem = new RepairItem();

            repairItem.CustomerRequestId = requestId;
            repairItem.Status = RepairItemStatus.Draft;

            await _context.RepairItems.AddAsync(repairItem);
            await _context.SaveChangesAsync();

            return repairItem;
        }

        public async void DeleteRepairItem(int id)
        {
            var repairItem = await _context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new DirectoryNotFoundException("Repair item not found");

            _context.RepairItems.Remove(repairItem);
            await _context.SaveChangesAsync();
        }

        public async Task<RepairItem> ApproveRepairItem(int id)
        {
            var repairItem = await _context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new DirectoryNotFoundException("Repair item not found");

            repairItem.Status = RepairItemStatus.OnHold;

            await _context.SaveChangesAsync();

            return repairItem;
        }

        public async Task<RepairItem> CompleteRepairItem(int id)
        {
            var repairItem = await _context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new DirectoryNotFoundException("Repair item not found");

            repairItem.Status = RepairItemStatus.Completed;

            await _context.SaveChangesAsync();

            return repairItem;
        }

        public async Task<RepairItem> CancelRepairItem(int id)
        {
            var repairItem = await _context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new DirectoryNotFoundException("Repair item not found");

            repairItem.Status = RepairItemStatus.Cancelled;

            await _context.SaveChangesAsync();

            return repairItem;
        }

        public async Task<RepairItem> AllowPickUpRepairItem(int id)
        {
            var repairItem = await _context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new DirectoryNotFoundException("Repair item not found");

            repairItem.Status = RepairItemStatus.WaitingForPickUp;

            await _context.SaveChangesAsync();

            return repairItem;
        }

        public async Task<RepairItem> WaitForRepairItemParts(int id)
        {
            var repairItem = await _context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new DirectoryNotFoundException("Repair item not found");

            repairItem.Status = RepairItemStatus.WaitingForParts;

            await _context.SaveChangesAsync();

            return repairItem;
        }

        public async Task<RepairItem> SetOnHoldRepairItemWork(int id)
        {
            var repairItem = await _context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new DirectoryNotFoundException("Repair item not found");

            repairItem.Status = RepairItemStatus.OnHold;

            await _context.SaveChangesAsync();

            return repairItem;
        }

        public async Task<RepairItem> EditRepairItem(int id, EditRepairItemDto dto)
        {
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
    }
}
