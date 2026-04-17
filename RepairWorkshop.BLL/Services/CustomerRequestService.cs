using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Entities;
using RepairWorkShop.DAL.Enums;


namespace RepairWorkshop.BLL.Services
{
    public class CustomerRequestService(
        AppDbContext context
    ) : ICustomerRequestService
    {
        public async Task<CustomerRequest> CreateRequest(/*int managerId, */CreateCustomerRequestDto dto)
        {
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

        public async void DeleteRequest(int id)
        {
            var request = await context.Requests.FindAsync(id);

            if (request == null)
                throw new NotFoundException("Request not found");

            if (request.Status != RequestStatus.Draft)
                throw new NotFoundException("Only draft can be deleted");

            //if (request.RepairItems.Any())
            //    throw new ConflictException("Cannot delete object with subobjects");

            context.Requests.Remove(request);

            await context.SaveChangesAsync();
        }

        public async Task<CustomerRequest> CancelRequest(int id)
        {
            var request = await context.Requests.FindAsync(id);

            if (request.Status == RequestStatus.Draft)
                throw new ConflictException("Request must be new or highter(not draft)");

            if (request == null)
                throw new NotFoundException("Request not found");

            // ТРЕБА ЗРОБИТИ ПО-ІНШОМУ
            request.Status = RequestStatus.Cancelled;

            foreach (var item in request.RepairItems)
            {
                item.Status = RepairItemStatus.Cancelled;

                foreach (var task in item.ServiceTasks)
                    task.Status = ServiceTaskStatus.Cancelled;
            }

            await context.SaveChangesAsync();

            return request;
        }

        public async Task<CustomerRequest> CompleteRequest(int id)
        {
            var request = await context.Requests.FindAsync(id);

            if (request.Status == RequestStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            if (request == null)
                throw new NotFoundException("Request not found");

            request.Status = RequestStatus.Completed;

            await context.SaveChangesAsync();

            return request;
        }

        public async Task<CustomerRequest> ApproveRequest(int id)
        {
            var request = await context.Requests.FindAsync(id);

            if (request.Status == RequestStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            if (request == null)
                throw new NotFoundException("Request not found");

            request.Status = RequestStatus.Approved;

            await context.SaveChangesAsync();

            return request;
        }

        public async Task<CustomerRequest> AllowPickUp(int id)
        {
            var request = await context.Requests.FindAsync(id);

            if (request.Status == RequestStatus.Draft)
                throw new ConflictException("Not allowed in draft");

            if (request == null)
                throw new NotFoundException("Request not found");

            request.Status = RequestStatus.WaitingForPickUp;

            await context.SaveChangesAsync();

            return request;
        }

        public async Task<CustomerRequest?> GetRequestById(int id)
        {
            return await context.Requests.FindAsync(id);
        }

        public async Task<List<CustomerRequest>> GetAllRequests()
        {
            return await context.Requests.ToListAsync();
        }

        public async Task<CustomerRequest> EditRequest(int id, EditCustomerRequestDto dto)
        {
            var request = await context.Requests.FindAsync(id);

            if (request == null)
                throw new NotFoundException("Request not found");

            request.CustomerId = dto.CustomerId;

            await context.SaveChangesAsync();

            return request;
        }

        public async Task<CustomerRequest> AssingCustomer(int id, int customerId)
        {
            var request = await context.Requests.FindAsync(id);

            if (request == null)
                throw new NotFoundException("Request not found");

            request.CustomerId = customerId;

            await context.SaveChangesAsync();

            return request;
        }

        public async Task<CustomerRequest> StartRequest(int id)
        {
            var request = await context.Requests.FindAsync(id);

            if (request == null)
                throw new NotFoundException("Request not found");

            if (request.Status != RequestStatus.Draft)
                throw new ConflictException("Allowed only in draft");

            if(request.RepairItems.Any() == false)
                throw new ConflictException("No items in request");

            request.Status = RequestStatus.New;

            context.SaveChangesAsync();

            return request;
        }
        //public async Task<CustomerRequest> AddItem(int id)
        //{
        //    var request = await context.Requests.FindAsync(id);

        //    if (request == null)
        //        throw new NotFoundException("Request not found");

        //    if (request.Status == RequestStatus.Draft)
        //        throw new ConflictException("Not allowed in draft");

        //    var repairItem = await context.RepairItems.FindAsync(id);

        //    request.RepairItems.Add
        //}
    }
}
