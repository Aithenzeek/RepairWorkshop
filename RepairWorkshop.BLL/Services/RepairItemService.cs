using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using RepairWorkShop.DAL.Enums;

namespace RepairWorkshop.BLL.Services
{
    public class RepairItemService
    {
        private readonly AppDbContext _context;

        public RepairItemService(AppDbContext context)
        {
            _context = context;
        }

        public RepairItem CreateRepairItem(int requestId)
        {
            var repairItem = new RepairItem();

            repairItem.CustomerRequestId = requestId;
            repairItem.Status = RepairItemStatus.Draft;

            _context.RepairItems.Add(repairItem);
            _context.SaveChanges();

            return repairItem;
        }

        public void DeleteRepairItem(int id)
        {
            var repairItem = _context.RepairItems.Find(id);

            if (repairItem == null)
                throw new Exception("Repair item not found");

            _context.RepairItems.Remove(repairItem);
            _context.SaveChanges();
        }

        public RepairItem ApproveRepairItem(int id)
        {
            var repairItem = _context.RepairItems.Find(id);

            if (repairItem == null)
                throw new Exception("Repair item not found");

            repairItem.Status = RepairItemStatus.OnHold;

            _context.SaveChanges();

            return repairItem;
        }

        public RepairItem CompleteRepairItem(int id)
        {
            var repairItem = _context.RepairItems.Find(id);

            if (repairItem == null)
                throw new Exception("Repair item not found");

            repairItem.Status = RepairItemStatus.Completed;

            _context.SaveChanges();

            return repairItem;
        }

        public RepairItem CancelRepairItem(int id)
        {
            var repairItem = _context.RepairItems.Find(id);

            if (repairItem == null)
                throw new Exception("Repair item not found");

            repairItem.Status = RepairItemStatus.Cancelled;

            _context.SaveChanges();

            return repairItem;
        }

        public RepairItem AllowPickUpRepairItem(int id)
        {
            var repairItem = _context.RepairItems.Find(id);

            if (repairItem == null)
                throw new Exception("Repair item not found");

            repairItem.Status = RepairItemStatus.WaitingForPickUp;

            _context.SaveChanges();

            return repairItem;
        }

        public RepairItem WaitForRepairItemParts(int id)
        {
            var repairItem = _context.RepairItems.Find(id);

            if (repairItem == null)
                throw new Exception("Repair item not found");

            repairItem.Status = RepairItemStatus.WaitingForParts;

            _context.SaveChanges();

            return repairItem;
        }

        public RepairItem SetOnHoldRepairItemWork(int id)
        {
            var repairItem = _context.RepairItems.Find(id);

            if (repairItem == null)
                throw new Exception("Repair item not found");

            repairItem.Status = RepairItemStatus.OnHold;

            _context.SaveChanges();

            return repairItem;
        }

        public RepairItem EditRepairItem(int id, CreateRepairItemDto dto)
        {
            var repairItem = _context.RepairItems.Find(id);

            if (repairItem == null)
                throw new Exception("Repair item not found");

            repairItem.Model = dto.Model;
            repairItem.SerialNumber = dto.SerialNumber;
            repairItem.ProblemDescription = dto.ProblemDescription;
            repairItem.Notes = dto.Notes;

            _context.SaveChanges();

            return repairItem;
        }
    }
}
