using System;
using System.Collections.Generic;
using System.Text;

namespace RepairWorkShop.DAL
{
    internal enum ServiceTaskStatus
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
