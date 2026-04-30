using RepairWorkShop.DAL.Enums;

namespace RepairWorkShop.DAL.Entities
{
    public class CustomerRequest
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? ManagerId { get; set; }
        public DateTime? StartedAt { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime? CompletedAt { get; set; }
        public double TotalCost { get; set; }
        public List<RepairItem> RepairItems { get; set; } = new();
        public Customer? Customer { get; set; }

        public void Cancel()
        {
            Status = RequestStatus.Cancelled;

            foreach (var item in RepairItems)
                item.Cancel();
        }

        public void Complete()
        {
            Status = RequestStatus.Completed;
        }

        public void SetOnHold()
        {
            Status = RequestStatus.OnHold;
        }

        public void WaitForParts()
        {
            Status = RequestStatus.WaitingForParts;
        }
    }
}
