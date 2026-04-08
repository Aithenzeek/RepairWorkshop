using System;
using System.Collections.Generic;
using System.Text;

using RepairWorkShop.DAL;

namespace RepairWorkshop.BLL.Services
{
    public class CustomerRequestService
    {
        private readonly AppDbContext _context;

        public CustomerRequestService(AppDbContext context)
        {
                _context = context;
        }

        public CustomerRequest CreateRequest()
        {
            var customerRequest = new CustomerRequest();

            _context.Requests.Add(customerRequest);
            _context.SaveChanges();

            return customerRequest;
        }
    }
}
