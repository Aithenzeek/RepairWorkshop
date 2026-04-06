using System;
using System.Collections.Generic;
using System.Text;

namespace RepairWorkShop.DAL
{
    internal class RepairItem
    {
        public int Id { get; set; }

        public string Model { get; set; }

        public string SerialNumber { get; set; }

        public string ProblemDescription { get; set; }

        public string Notes { get; set; }

        public RepairItemStatus Status { get; set; }

        public double ServiceCost { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public List<ServiceTask> ServiceTasks { get; set; } = new();
    }
}
