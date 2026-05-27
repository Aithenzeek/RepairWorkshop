namespace RepairWorkShop.DAL.Enums
{
    public enum RequestStatus
    {
        Draft,
        New,
        InProgress,
        OnHold,
        WaitingForParts,
        CompletedByTechnician,
        Completed,
        Cancelled,
        WaitingForPickUp,
        PickedUp
    }
}
