using RepairWorkShop.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepairWorkShop.DAL.Entities
{
    public class RepairItem
    {
        public int Id { get; set; }
        public int CustomerRequestId { get; set; }
        public string Model { get; set; } = null!;
        public string SerialNumber { get; set; } = null!;
        public string ProblemDescription { get; set; } = null!;
        public string Notes { get; set; } = null!;
        public RepairItemStatus Status { get; set; }
        public double ServiceCost { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public CustomerRequest CustomerRequest { get; set; } = null!;
        public List<ServiceTask> ServiceTasks { get; set; } = [];
    }
}
