using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using RepairWorkShop.DAL.Enums;
using static System.Net.WebRequestMethods;


namespace RepairWorkshop.BLL.Services
{
    public class CustomerRequestService(
        AppDbContext context
    ) : ICustomerRequestService
    {
        public async Task<ResponseCustomerRequestDto> CreateRequest(CreateCustomerRequestDto dto, int managerId)
        {
            var existingCustomer = await context.Customers.FindAsync(dto.CustomerId) ?? throw new NotFoundException("Customer not found");
            var existingManager = await context.Users.Include(m => m.Role).FirstOrDefaultAsync(m => m.Id == managerId) ?? throw new NotFoundException("Manager not found");

            if (existingManager.Role.Name != "Manager" && existingManager.Role.Name != "Superadmin")
                throw new BadRequestException("Selected user is not manager"); // TODO: або тільки для менеджера або для нього і супер адміна

            var customerRequest = new CustomerRequest
            {
                CustomerId = dto.CustomerId,
                ManagerId = managerId,
                Status = RequestStatus.Draft
            };

            await context.Requests.AddAsync(customerRequest);

            await context.SaveChangesAsync();

            return await ReturnDto(customerRequest);
        }

        public async Task DeleteRequest(int id)
        {
            var request = await context.Requests.FindAsync(id) ?? throw new NotFoundException("Request not found");

            if (request.Status != RequestStatus.Draft)
                throw new ConflictException("Only draft can be deleted");

            context.Requests.Remove(request);

            await context.SaveChangesAsync();
        }

        public async Task<ResponseCustomerRequestDto> CancelRequest(int id, CancelCustomerRequestDto dto)
        {
            var request = await context.Requests.Include(r => r.Customer).Include(r => r.Manager).Include(r => r.RepairItems).ThenInclude(s => s.ServiceTasks).FirstOrDefaultAsync(r => r.Id == id) ?? throw new NotFoundException("Request not found");

            if (request.Status == RequestStatus.Draft || request.Status == RequestStatus.Completed)
                throw new ConflictException("Not allowed in draft or completed");

            request.Cancel();

            request.CancellationReason = dto.CancellationReason;

            await context.SaveChangesAsync();

            return await ReturnDto(request);
        }

        public async Task<ResponseCustomerRequestDto> CompleteRequest(int id)
        {
            var request = await context.Requests.Include(r => r.Customer).Include(r => r.Manager).Include(r => r.RepairItems).FirstOrDefaultAsync(r => r.Id == id) ?? throw new NotFoundException("Request not found");

            if (request.Status != RequestStatus.CompletedByTechnician || request.RepairItems.All(r => r.Status != RepairItemStatus.Completed && r.Status != RepairItemStatus.Cancelled))
                throw new ConflictException("Allowed when all repair items completed or cancelled");

            request.Complete();

            await context.SaveChangesAsync();

            return await ReturnDto(request);
        }

        //public async Task<CustomerRequest> ApproveRequest(int id)
        //{
        //    var request = await context.Requests.FindAsync(id);

        //    if (request == null)
        //        throw new NotFoundException("Request not found");

        //    if (request.Status == RequestStatus.Draft)
        //        throw new ConflictException("Not allowed in draft");

        //    request.Status = RequestStatus.Approved;

        //    await context.SaveChangesAsync();

        //    return request;
        //}

        public async Task<ResponseCustomerRequestDto> AllowPickUp(int id)
        {
            var request = await context.Requests.Include(r => r.Customer).Include(r => r.Manager).Include(r => r.RepairItems).FirstOrDefaultAsync(r => r.Id == id) ?? throw new NotFoundException("Request not found");

            if (request.Status == RequestStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            if (request.RepairItems.All(r => r.Status != RepairItemStatus.Cancelled && r.Status != RepairItemStatus.Completed))
                throw new ConflictException("Can be allowed to pick up when all items cancelled or completed");

            request.AllowPickUp();

            await context.SaveChangesAsync();

            return await ReturnDto(request);
        }

        public async Task<CustomerRequest?> GetRequestById(int id)
        {
            return await context.Requests
                .AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.Manager)
                .FirstOrDefaultAsync(c => c.Id == id) ?? throw new NotFoundException("Request not found");
        }

        public async Task<List<CustomerRequest>> GetAllRequests()
        {
            return await context.Requests
                .AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.Manager)
                .ToListAsync();
        }

        public async Task<ResponseCustomerRequestDto> EditRequest(int id, EditCustomerRequestDto dto)
        {
            var existingCustomer = await context.Customers.FindAsync(dto.CustomerId) ?? throw new NotFoundException("Customer not found");

            var request = await context.Requests.Include(r => r.Customer).Include(r => r.Manager).FirstOrDefaultAsync(r => r.Id == id) ?? throw new NotFoundException("Request not found");

            request.CustomerId = dto.CustomerId;

            await context.SaveChangesAsync();

            return await ReturnDto(request);
        }

        //public async Task<CustomerRequest> AssingCustomer(int id, int customerId)
        //{
        //    var request = await context.Requests.FindAsync(id);

        //    if (request == null)
        //        throw new NotFoundException("Request not found");

        //    request.CustomerId = customerId;

        //    await context.SaveChangesAsync();

        //    return request;
        //}
        // переробити
        public async Task<ResponseCustomerRequestDto> StartRequest(int id) // TODO: може забрати якшо робити, шо воно буде через ітем йти
        {
            var request = await context.Requests.Include(r => r.Customer).Include(r => r.Manager).Include(r => r.RepairItems).FirstOrDefaultAsync(r => r.Id == id)
                ?? throw new NotFoundException("Request not found");

            // invalidState по статусу валідацію, загальна помилка 
            if (request.Status != RequestStatus.Draft)
                throw new BadRequestException("Allowed only in draft"); // не можна робити дію

            if (!request.RepairItems.Any())
                throw new ConflictException("No items in request");

            request.Start();

            await context.SaveChangesAsync();

            return await ReturnDto(request);
        }

        public async Task<ResponseCustomerRequestDto> ReturnDto(CustomerRequest request)
        {
            return new ResponseCustomerRequestDto(
                request.Id,
                request.CustomerId,
                request.ManagerId,
                request.StartedAt,
                request.Status,
                request.CompletedAt,
                request.TotalCost,
                request.CancellationReason,
                request.Customer.Name,
                request.Manager.Name
            );
        }

        public async Task<PagedResponse<ResponseCustomerRequestDto>> GetPaged(RequestFilterDto filter)
        {
            var query = context.Requests
                .Include(x => x.Customer)
                .Include(x => x.Manager)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                query = query.Where(x => x.Status.ToString() == filter.Status);
            }

            if (filter.DateFrom.HasValue)
            {
                query = query.Where(x => x.StartedAt >= filter.DateFrom);
            }

            if (filter.DateTo.HasValue)
            {
                query = query.Where(x => x.StartedAt <= filter.DateTo);
            }

            query = filter.SortBy?.ToLower() switch
            {
                "date" => query.OrderByDescending(x => x.StartedAt),

                "status" => query.OrderBy(x => x.Status),

                _ => query.OrderByDescending(x => x.StartedAt)
            };

            var total = await query.CountAsync();

            var items = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new ResponseCustomerRequestDto(
                    x.Id,
                    x.CustomerId,
                    x.ManagerId,
                    x.StartedAt,    
                    x.Status,
                    x.CompletedAt,
                    x.TotalCost,
                    x.CancellationReason,
                    x.Customer.Name,
                    x.Manager.Name
                ))
                .ToListAsync();

            return new PagedResponse<ResponseCustomerRequestDto>
            {
                Items = items,
                TotalCount = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }
    }
}
