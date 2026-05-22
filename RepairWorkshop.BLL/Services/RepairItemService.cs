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
            var request = await context.Requests.FindAsync(dto.CustomerRequestId) ?? throw new NotFoundException("request not found");
            
            if (request.Status != RequestStatus.Draft && request.Status != RequestStatus.CompletedByTechnician)
                throw new ConflictException("Can add only to draft or completed by technician request"); // TODO: ше може зробити перевірку чи є такий ітем, але сенсу мало, бо він створюється тільки з статусом і ід реквеста

            var repairItem = new RepairItem
            {
                CustomerRequestId = dto.CustomerRequestId,
                Status = RepairItemStatus.Draft,
            };

            if (request.Status != RequestStatus.Draft)
            request.Status = RequestStatus.OnHold;
            request.CompletedAt = null;

            await context.RepairItems.AddAsync(repairItem);
            await context.SaveChangesAsync();

            return await ReturnDto(repairItem);
        }

        public async Task DeleteRepairItem(int id)
        {
            var request = await context.Requests.Include(r => r.RepairItems).FirstOrDefaultAsync(r => r.RepairItems.Any(r => r.Id == id)) ?? throw new NotFoundException("request not found");

            //var repairItem = await context.RepairItems.FindAsync(id);
            var repairItem = request.RepairItems.FirstOrDefault(r => r.Id == id) ?? throw new NotFoundException("Repair item not found");
            
            if (repairItem.Status != RepairItemStatus.Draft)
                throw new ConflictException("Only draft can be deleted");

            request.RepairItems.Remove(repairItem);

            if (request.RepairItems.All(r => r.Status == RepairItemStatus.Completed || r.Status == RepairItemStatus.Cancelled || r.Status == RepairItemStatus.CompletedByTechnician))
                request.Status = RequestStatus.CompletedByTechnician;

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

            if (repairItem.ServiceTasks.Count == 0)
                throw new NotFoundException("Can`t be started without any service task");

            if (repairItem.Model == null || repairItem.Notes == null || repairItem.ProblemDescription == null || repairItem.SerialNumber == null)
                throw new BadRequestException("Not all data filled");

            //if (repairItem.Status == RepairItemStatus.New)
            //    throw new ConflictException("Repair item already started");

            //if (repairItem.Status != RepairItemStatus.New)
            //    throw new ConflictException("Only new can be started");

            repairItem.Start();

            if (request.RepairItems.Any(r => r.Status == RepairItemStatus.New))
                request.Status = RequestStatus.New;

            await context.SaveChangesAsync();

            return await ReturnDto(repairItem);
        }

        public async Task<ResponseRepairItemDto> CompleteRepairItem(int id) // видалити напевно або тільки зробити чисто для того шоб з виконаного техніком перевелося в виконане
        {
            var request = await context.Requests.Include(r => r.RepairItems).FirstOrDefaultAsync(r => r.RepairItems.Any(r => r.Id == id)) ?? throw new NotFoundException("Request not found");

            var repairItem = request.RepairItems.FirstOrDefault(r => r.Id == id) ?? throw new NotFoundException("Repair item not found");

            if (repairItem.Status == RepairItemStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            if (repairItem.Status != RepairItemStatus.CompletedByTechnician)
                throw new BadRequestException("Allowed when completed by technician");

            repairItem.Complete();

            if (request.RepairItems.All(r => r.Status == RepairItemStatus.Completed || r.Status == RepairItemStatus.Cancelled))
                request.Status = RequestStatus.CompletedByTechnician;

            await context.SaveChangesAsync();

            return await ReturnDto(repairItem);
        }

        public async Task<ResponseRepairItemDto> CancelRepairItem(int id, CancelRepairItemDto dto)
        {
            var request = await context.Requests.Include(r => r.RepairItems).FirstOrDefaultAsync(r => r.RepairItems.Any(r => r.Id == id)) ?? throw new NotFoundException("Request not found");

            var repairItem = request.RepairItems.FirstOrDefault(r => r.Id == id) ?? throw new NotFoundException("Repair item not found");

            if (repairItem.Status == RepairItemStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            repairItem.Cancel();

            repairItem.CancellationReason = dto.CancellationReason;

            if (request.RepairItems.All(r => r.Status == RepairItemStatus.Cancelled))
                request.Status = RequestStatus.OnHold;

            await context.SaveChangesAsync();

            return await ReturnDto(repairItem);
        }

        public async Task<ResponseRepairItemDto> AllowPickUpRepairItem(int id)
        {
            var request = await context.Requests.Include(r => r.RepairItems).FirstOrDefaultAsync(r => r.RepairItems.Any(r => r.Id == id)) ?? throw new NotFoundException("Request not found");

            var repairItem = request.RepairItems.FirstOrDefault(r => r.Id == id) ?? throw new NotFoundException("Repair item not found");

            if (repairItem.Status == RepairItemStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            if (repairItem.Status != RepairItemStatus.Completed && repairItem.Status != RepairItemStatus.Cancelled)
                throw new ConflictException("Allowed only for completed or cancelled");

            repairItem.AllowPickUp();

            if (request.RepairItems.Any(r => r.Status == RepairItemStatus.WaitingForPickUp))
                request.Status = RequestStatus.WaitingForPickUp;

            await context.SaveChangesAsync();

            return await ReturnDto(repairItem);
        }

        public async Task<ResponseRepairItemDto> WaitForRepairItemParts(int id) //TODO: тут цього напевно не треба, бо воно буде братися з тасків
        {
            var repairItem = await context.RepairItems.FindAsync(id) ?? throw new NotFoundException("Repair item not found");
            
            if (repairItem.Status == RepairItemStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            repairItem.Status = RepairItemStatus.WaitingForParts;

            await context.SaveChangesAsync();

            return await ReturnDto(repairItem);
        }

        public async Task<ResponseRepairItemDto> SetOnHoldRepairItemWork(int id) // TODO: може забрати, бо автоматично з таска йде
        {
            var repairItem = await context.RepairItems.FindAsync(id) ?? throw new NotFoundException("Repair item not found");
            
            if (repairItem.Status == RepairItemStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            repairItem.Status = RepairItemStatus.OnHold;

            await context.SaveChangesAsync();

            return await ReturnDto(repairItem);
        }

        public async Task<ResponseRepairItemDto> EditRepairItem(int id, EditRepairItemDto dto)
        {
            var serialNumber = await context.RepairItems.AnyAsync(x => x.SerialNumber == dto.SerialNumber); //TODO: переробити, бо не вийде змінити його(не треба міняти, нормально робе)

            var repairItem = await context.RepairItems.FindAsync(id) ?? throw new NotFoundException("Repair item not found");
            
            if (serialNumber && repairItem.SerialNumber != dto.SerialNumber)
                throw new ConflictException("Item with such serial number exists");


            repairItem.Model = dto.Model;
            repairItem.SerialNumber = dto.SerialNumber;
            repairItem.ProblemDescription = dto.ProblemDescription;
            repairItem.Notes = dto.Notes;

            await context.SaveChangesAsync();

            return await ReturnDto(repairItem);
        }

        public async Task<RepairItem?> GetRepairItemById(int id)
        {
            return await context.RepairItems
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id) ?? throw new NotFoundException("Repair item not found");
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

        public async Task<ResponseRepairItemDto> ReturnDto(RepairItem repairItem)
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
                repairItem.CompletedAt,
                repairItem.CancellationReason
            );
        }

        public async Task<PagedResponse<ResponseRepairItemDto>> GetPaged(RepairItemFilterDto filter)
        {
            var query = context.RepairItems.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                query = query.Where(x => x.Status.ToString() == filter.Status);
            }

            if (filter.DateFrom.HasValue)
            {
                query = query.Where(x => x.StartedAt >= filter.DateFrom.Value);
            }

            if (filter.DateTo.HasValue)
            {
                query = query.Where(x => x.StartedAt <= filter.DateTo.Value);
            }

            if (filter.CustomerRequestId.HasValue)
            {
                query = query.Where(x => x.CustomerRequestId == filter.CustomerRequestId.Value);
            }

            query = filter.SortBy?.ToLower() switch
            {
                "dateasc" => query.OrderBy(x => x.StartedAt),
                "datedesc" => query.OrderByDescending(x => x.StartedAt),

                "costasc" => query.OrderBy(x => x.ServiceCost),
                "costdesc" => query.OrderByDescending(x => x.ServiceCost),

                "requestasc" => query.OrderBy(x => x.CustomerRequestId).ThenBy(x => x.Id),

                "requestdesc" => query.OrderByDescending(x => x.CustomerRequestId).ThenBy(x => x.Id),

                _ => query.OrderByDescending(x => x.StartedAt)
            };

            var total = await query.CountAsync();

            var items = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new ResponseRepairItemDto(
                    x.Id,
                    x.CustomerRequestId,
                    x.Model,
                    x.SerialNumber,
                    x.ProblemDescription,
                    x.Notes,
                    x.Status,
                    x.ServiceCost,
                    x.StartedAt,
                    x.CompletedAt,
                    x.CancellationReason
                ))
                .ToListAsync();

            return new PagedResponse<ResponseRepairItemDto>
            {
                Items = items,
                TotalCount = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<PagedResponse<ResponseRepairItemDto>> GetActivePaged(int userId, bool activeOnly, int page = 1, int pageSize = 10)
        {
            var query = context.ServiceTasks.Where(q => q.UserId == userId).Select(q => q.RepairItem);

            if (activeOnly)
            {
                query = query.Where(r => r.Status != RepairItemStatus.Draft &&
                    r.Status != RepairItemStatus.Completed &&
                    r.Status != RepairItemStatus.Cancelled &&
                    r.Status != RepairItemStatus.CompletedByTechnician &&
                    r.Status != RepairItemStatus.WaitingForPickUp);
            }

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ResponseRepairItemDto(
                    x.Id,
                    x.CustomerRequestId,
                    x.Model,
                    x.SerialNumber,
                    x.ProblemDescription,
                    x.Notes,
                    x.Status,
                    x.ServiceCost,
                    x.StartedAt,
                    x.CompletedAt,
                    x.CancellationReason
                ))
                .ToListAsync();

            return new PagedResponse<ResponseRepairItemDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
