using RepairWorkShop.DAL.Enums;

namespace RepairWorkShop.DAL.Entities
{
    public class ServiceTask
    {
        public int Id { get; set; }
        public int RepairItemId { get; set; }
        public int WorkerId { get; set; }
        public int ServiceId { get; set; }
        public double Cost { get; set; }
        public ServiceTaskStatus Status { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public Service Service { get; set; } = null!;
        public RepairItem RepairItem { get; set; } = null!;
    }
}
