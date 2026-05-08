using RepairWorkShop.DAL.Enums;

namespace RepairWorkShop.DAL.Entities
{
    public class ServiceTask
    {
        public int Id { get; set; }
        public int RepairItemId { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public double? Cost { get; set; }
        public ServiceTaskStatus Status { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? DiagnosticsResult { get; set; }

        public Service Service { get; set; } = null!;
        public RepairItem RepairItem { get; set; } = null!;
        public User User { get; set; } = null!;

        public void Cancel()
        {
            Status = ServiceTaskStatus.Cancelled;

            if (StartedAt == null)
                StartedAt = DateTime.Now;

            CompletedAt = DateTime.Now;
        }

        public void Complete()
        {
            Status = ServiceTaskStatus.Completed;
            CompletedAt = DateTime.Now;
        }

        public void SetOnHold()
        {
            Status = ServiceTaskStatus.OnHold;
        }

        public void WaitForParts()
        {
            Status = ServiceTaskStatus.WaitingForParts;
        }

        public void Start()
        {
            Status = ServiceTaskStatus.InProgress;
        }
    }
}
