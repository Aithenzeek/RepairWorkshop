using RepairWorkShop.DAL.Enums;

namespace RepairWorkShop.DAL.Entities
{
    public class CustomerRequest : AuditableEntity
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? ManagerId { get; set; }
        public DateTime? StartedAt { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime? CompletedAt { get; set; }
        public double TotalCost { get; set; }
        public string? CancellationReason { get; set; }

        public List<RepairItem> RepairItems { get; set; } = new();
        public Customer? Customer { get; set; }
        public User? Manager { get; set; }

        public void Cancel()
        {
            Status = RequestStatus.Cancelled;
            if (StartedAt == null)
                StartedAt = DateTime.Now;
            if (CompletedAt == null)
                CompletedAt = DateTime.Now;

            foreach (var item in RepairItems)
                item.Cancel();
        }

        public void Complete()
        {
            Status = RequestStatus.Completed;
        }

        public void CompleteByTechnician()
        {
            Status = RequestStatus.CompletedByTechnician;
            CompletedAt = DateTime.Now;
        }

        public void SetOnHold()
        {
            Status = RequestStatus.OnHold;
        }

        public void WaitForParts()
        {
            Status = RequestStatus.WaitingForParts;
        }

        public void AllowPickUp()
        {
            Status = RequestStatus.WaitingForPickUp;

            foreach (var item in RepairItems)
                item.Status = RepairItemStatus.WaitingForPickUp;
        }

        public void Start()
        {
            Status = RequestStatus.New;

            foreach (var item in RepairItems)
                item.Status = RepairItemStatus.New;
        }

        public void GetTotalCost()
        {
            TotalCost = 0;

            foreach (var repairItem in RepairItems)
                TotalCost += repairItem.ServiceCost ?? 0;
        }
    }
}
