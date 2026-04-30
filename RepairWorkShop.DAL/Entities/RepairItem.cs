using RepairWorkShop.DAL.Enums;

namespace RepairWorkShop.DAL.Entities
{
    public class RepairItem
    {
        public int Id { get; set; }
        public int CustomerRequestId { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public string? ProblemDescription { get; set; }
        public string? Notes { get; set; }
        public RepairItemStatus Status { get; set; }
        public double? ServiceCost { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public CustomerRequest CustomerRequest { get; set; } = null!;
        public List<ServiceTask> ServiceTasks { get; set; } = [];

        public void Cancel()
        {
            Status = RepairItemStatus.Cancelled;

            foreach (var task in ServiceTasks)
                task.Cancel();
        }

        public void Complete()
        {
            Status = RepairItemStatus.Completed;
        }

        public void SetOnHold()
        {
            Status = RepairItemStatus.OnHold;
        }

        public void WaitForParts()
        {
            Status = RepairItemStatus.WaitingForParts;
        }
    }
}
