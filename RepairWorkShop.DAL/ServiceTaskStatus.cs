using System;
using System.Collections.Generic;
using System.Text;

namespace RepairWorkShop.DAL
{
    public enum ServiceTaskStatus
    {
        New,
        Assigned,
        InProgress,
        OnHold,
        WaitingForParts,
        Completed,
        Cancelled
    }
}
