using Microsoft.EntityFrameworkCore;
using RepairWorkshop.BLL.DTOs;
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
        public async Task<CustomerRequest> CreateRequest(int managerId)
        {
            var customerRequest = new CustomerRequest
            {
                ManagerId = managerId,
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
                throw new DirectoryNotFoundException("Request not found");

            context.Requests.Remove(request);

            await context.SaveChangesAsync();
        }

        public async Task<CustomerRequest> CancelRequest(int id)
        {
            var request = await context.Requests.FindAsync(id);

            if (request == null)
                throw new DirectoryNotFoundException("Request not found");

            request.Status = RequestStatus.Cancelled;

            await context.SaveChangesAsync();

            return request;
        }

        public async Task<CustomerRequest> CompleteRequest(int id)
        {
            var request = await context.Requests.FindAsync(id);

            if (request == null)
                throw new DirectoryNotFoundException("Request not found");

            request.Status = RequestStatus.Completed;

            await context.SaveChangesAsync();

            return request;
        }

        public async Task<CustomerRequest> ApproveRequest(int id)
        {
            var request = await context.Requests.FindAsync(id);

            if (request == null)
                throw new DirectoryNotFoundException("Request not found");

            request.Status = RequestStatus.Approved;

            await context.SaveChangesAsync();

            return request;
        }

        public async Task<CustomerRequest> AllowPickUp(int id)
        {
            var request = await context.Requests.FindAsync(id);

            if (request == null)
                throw new DirectoryNotFoundException("Request not found");

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

        public async Task<CustomerRequest> EditRequest(int id, CreateCustomerRequestDto dto)
        {
            var request = await context.Requests.FindAsync(id);

            if (request == null)
                throw new DirectoryNotFoundException("Request not found");

            request.CustomerId = dto.CustomerId;

            await context.SaveChangesAsync();

            return request;
        }

        public async Task<CustomerRequest> AssingCutomer(int id, int customerId)
        {
            var request = await context.Requests.FindAsync(id);

            if (request == null)
                throw new DirectoryNotFoundException("Request not found");

            request.CustomerId = customerId;

            await context.SaveChangesAsync();

            return request;
        }
    }
}
