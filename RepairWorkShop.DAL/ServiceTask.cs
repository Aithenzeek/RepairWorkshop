using System;
using System.Collections.Generic;
using System.Text;

namespace RepairWorkShop.DAL
{
    internal class ServiceTask
    {
        public int Id { get; set; }
        public int RepairItemId { get; set; }
        public int WorkerId { get; set; }
        public double Cost { get; set; }
        public Service Service { get; set; }
        public ServiceTaskStatus Status {  get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
