using System;
using System.Collections.Generic;
using System.Text;

namespace RepairWorkShop.DAL
{
    public enum RepairItemStatus
    {
        New,
        Assigned,
        InProgress,
        OnHold,
        WaitingForParts,
        Completed,
        WaitingForPickUp,
        Cancelled
    }
}
