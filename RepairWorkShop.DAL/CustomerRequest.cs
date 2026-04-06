using System;
using System.Collections.Generic;
using System.Text;

namespace RepairWorkShop.DAL
{
    internal class CustomerRequest
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int ManagerId { get; set; }

        public DateTime CreatedAt { get; set; }

        public RequestStatus Status { get; set; }

        public DateTime? CompletedAt { get; set; }

        public double TotalCost { get; set; }

        public List<RepairItem> RepairItems { get; set; } = new();
    }
}
