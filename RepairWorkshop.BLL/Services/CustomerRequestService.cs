using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using RepairWorkShop.DAL.Enums;
using System.Diagnostics.CodeAnalysis;


namespace RepairWorkshop.BLL.Services
{
    public class CustomerRequestService(
        AppDbContext context
    ) : ICustomerRequestService
    {
        public async Task<CustomerRequest> CreateRequest(CreateCustomerRequestDto dto)
        {
            var existingManager = await context.Users.Include(m => m.Role).FirstOrDefaultAsync(m => m.Id == dto.ManagerId);

            if (existingManager == null)
                throw new NotFoundException("Manager not found");

            if (existingManager.Role.Name != "Manager" && existingManager.Role.Name != "Superadmin")
                throw new BadRequestException("Selected user is not manager"); // TODO: або тільки для менеджера або для нього і супер адміна

            var customerRequest = new CustomerRequest
            {
                CustomerId = dto.CustomerId,
                ManagerId = dto.ManagerId,
                Status = RequestStatus.Draft
            };

            await context.Requests.AddAsync(customerRequest);

            await context.SaveChangesAsync();

            return customerRequest;
        }

        public async Task DeleteRequest(int id)
        {
            var request = await context.Requests.FindAsync(id);

            if (request == null)
                throw new NotFoundException("Request not found");

            if (request.Status != RequestStatus.Draft)
                throw new ConflictException("Only draft can be deleted");

            context.Requests.Remove(request);

            await context.SaveChangesAsync();
        }

        public async Task<CustomerRequest> CancelRequest(int id)
        {
            var request = await context.Requests.FindAsync(id);

            if (request == null)
                throw new NotFoundException("Request not found");

            if (request.Status == RequestStatus.Draft || request.Status == RequestStatus.Completed)
                throw new ConflictException("Not allowed in draft or completed");

            request.Cancel();

            await context.SaveChangesAsync();

            return request;
        }

        public async Task<CustomerRequest> CompleteRequest(int id)
        {
            var request = await context.Requests.FindAsync(id);

            if (request == null)
                throw new NotFoundException("Request not found");

            if (request.Status == RequestStatus.Draft || request.Status == RequestStatus.Cancelled)
                throw new ConflictException("Not allowed in draft or cancelled");

            request.Complete();

            await context.SaveChangesAsync();

            return request;
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

        public async Task<CustomerRequest> AllowPickUp(int id)
        {
            var request = await context.Requests.FindAsync(id);

            if (request == null)
                throw new NotFoundException("Request not found");

            if (request.Status == RequestStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            request.AllowPickUp();

            await context.SaveChangesAsync();

            return request;
        }

        public async Task<CustomerRequest?> GetRequestById(int id)
        {
            return await context.Requests
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<CustomerRequest>> GetAllRequests()
        {
            return await context.Requests
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CustomerRequest> EditRequest(int id, EditCustomerRequestDto dto)
        {
            var request = await context.Requests.FindAsync(id);

            if (request == null)
                throw new NotFoundException("Request not found");

            request.CustomerId = dto.CustomerId;
            request.ManagerId = dto.ManagerId;

            await context.SaveChangesAsync();

            return request;
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
        public async Task<CustomerRequest> StartRequest(int id)
        {
            var request = await context.Requests.FindAsync(id)
                ?? throw new NotFoundException("Request not found");

            // invalidState по статусу валідацію, загальна помилка 
            if (request.Status != RequestStatus.Draft)
                throw new BadRequestException("Allowed only in draft"); // не можна робити дію

            if(!request.RepairItems.Any())
                throw new ConflictException("No items in request");

            request.Start();

            await context.SaveChangesAsync();

            return request;
        }
    }
}
