using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using RepairWorkShop.DAL.Enums;

namespace RepairWorkshop.BLL.Services
{
    public class RepairItemService(AppDbContext context) : IRepairItemService
    {
        public async Task<ResponseRepairItemDto> CreateRepairItem(CreateRepairItemDto dto)
        {
            var request = await context.Requests.FindAsync(dto.CustomerRequestId);

            if (request == null)
                throw new NotFoundException("request not found");

            if (request.Status != RequestStatus.Draft)
                throw new ConflictException("Cannot add to non-draft request"); // TODO: ше може зробити перевірку чи є такий ітем, але сенсу мало, бо він створюється тільки з статусом і ід реквеста

            var repairItem = new RepairItem
            {
                CustomerRequestId = dto.CustomerRequestId,
                Status = RepairItemStatus.Draft,
            };

            await context.RepairItems.AddAsync(repairItem);
            await context.SaveChangesAsync();

            return ReturnDto(repairItem);
        }

        public async Task DeleteRepairItem(int id)
        {
            var repairItem = await context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new NotFoundException("Repair item not found");

            if (repairItem.Status != RepairItemStatus.Draft)
                throw new ConflictException("Only draft can be deleted");

            context.RepairItems.Remove(repairItem);

            await context.SaveChangesAsync();
        }

        //public async Task<RepairItem> ApproveRepairItem(int id)
        //{
        //    var repairItem = await context.RepairItems.FindAsync(id);

        //    if (repairItem == null)
        //        throw new NotFoundException("Repair item not found");

        //    if (repairItem.Status != RepairItemStatus.Draft)
        //        throw new ConflictException("Only draft can be approved");

        //    repairItem.Status = RepairItemStatus.Approved; //TODO: статус змінити(вже ніби змінив)

        //    await context.SaveChangesAsync();

        //    return repairItem;
        //}

        public async Task<ResponseRepairItemDto> StartRepairItem(int id)
        {
            var request = await context.Requests.Include(r => r.RepairItems).ThenInclude(s => s.ServiceTasks).FirstOrDefaultAsync(r => r.RepairItems.Any(r => r.Id == id)) ?? throw new NotFoundException("Request not found");

            var repairItem = request.RepairItems.FirstOrDefault(r => r.Id == id) ?? throw new NotFoundException("Repair item not found");

            //if (repairItem.Status == RepairItemStatus.Approved)
            //    throw new ConflictException("Only approved repair item can be started");

            repairItem.Start();

            if (request.RepairItems.Any(r => r.Status == RepairItemStatus.New))
                request.Status = RequestStatus.New;

            await context.SaveChangesAsync();

            return ReturnDto(repairItem);
        }

        public async Task<ResponseRepairItemDto> CompleteRepairItem(int id)
        {
            var repairItem = await context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new NotFoundException("Repair item not found");

            if (repairItem.Status == RepairItemStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            repairItem.Complete();

            await context.SaveChangesAsync();

            return ReturnDto(repairItem);
        }

        public async Task<ResponseRepairItemDto> CancelRepairItem(int id)
        {
            var repairItem = await context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new NotFoundException("Repair item not found");

            if (repairItem.Status == RepairItemStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            repairItem.Cancel();

            await context.SaveChangesAsync();

            return ReturnDto(repairItem);
        }

        public async Task<ResponseRepairItemDto> AllowPickUpRepairItem(int id)
        {
            var request = await context.Requests.Include(r => r.RepairItems).FirstOrDefaultAsync(r => r.RepairItems.Any(r => r.Id == id)) ?? throw new NotFoundException("Request not found");

            var repairItem = request.RepairItems.FirstOrDefault(r => r.Id == id) ?? throw new NotFoundException("Repair item not found");

            if (repairItem.Status == RepairItemStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            repairItem.AllowPickUp();

            if (request.RepairItems.Any(r => r.Status == RepairItemStatus.WaitingForPickUp))
                request.Status = RequestStatus.WaitingForPickUp;

            await context.SaveChangesAsync();

            return ReturnDto(repairItem);
        }

        public async Task<ResponseRepairItemDto> WaitForRepairItemParts(int id)
        {
            var repairItem = await context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new NotFoundException("Repair item not found");

            if (repairItem.Status == RepairItemStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            repairItem.Status = RepairItemStatus.WaitingForParts;

            await context.SaveChangesAsync();

            return ReturnDto(repairItem);
        }

        public async Task<ResponseRepairItemDto> SetOnHoldRepairItemWork(int id)
        {
            var repairItem = await context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new NotFoundException("Repair item not found");

            if (repairItem.Status == RepairItemStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            repairItem.Status = RepairItemStatus.OnHold;

            await context.SaveChangesAsync();

            return ReturnDto(repairItem);
        }

        public async Task<ResponseRepairItemDto> EditRepairItem(int id, EditRepairItemDto dto)
        {
            var serialNumber = await context.RepairItems.AnyAsync(x => x.SerialNumber == dto.SerialNumber); //TODO: переробити, бо не вийде змінити його(не треба міняти, нормально робе)

            if (serialNumber)
                throw new ConflictException("Item with such serial number exists");

            var repairItem = await context.RepairItems.FindAsync(id);

            if (repairItem == null)
                throw new NotFoundException("Repair item not found");

            repairItem.Model = dto.Model;
            repairItem.SerialNumber = dto.SerialNumber;
            repairItem.ProblemDescription = dto.ProblemDescription;
            repairItem.Notes = dto.Notes;

            await context.SaveChangesAsync();

            return ReturnDto(repairItem);
        }

        public async Task<RepairItem?> GetRepairItemById(int id)
        {
            return await context.RepairItems
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<RepairItem>> GetAllRepairItems()
        {
            return await context.RepairItems
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<RepairItem>> GetAllActiveRepairItems(int userId, bool activeOnly)
        {
            var repairItems = context.ServiceTasks
                    .Where(t => t.UserId == userId)
                    .Select(t => t.RepairItem);

            if (activeOnly)
            {
                repairItems = repairItems.Where(r => r.Status != RepairItemStatus.Draft &&
                    r.Status != RepairItemStatus.Completed &&
                    r.Status != RepairItemStatus.Cancelled &&
                    r.Status != RepairItemStatus.CompletedByTechnician &&
                    r.Status != RepairItemStatus.WaitingForPickUp);
            }

            return await repairItems.Distinct().ToListAsync();
        }

        public ResponseRepairItemDto ReturnDto(RepairItem repairItem)
        {
            return new ResponseRepairItemDto(
                repairItem.Id,
                repairItem.CustomerRequestId,
                repairItem.Model,
                repairItem.SerialNumber,
                repairItem.ProblemDescription,
                repairItem.Notes,
                repairItem.Status,
                repairItem.ServiceCost,
                repairItem.StartedAt,
                repairItem.CompletedAt
            );
        }
    }
}
