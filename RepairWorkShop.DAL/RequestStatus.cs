using System;
using System.Collections.Generic;
using System.Text;

namespace RepairWorkShop.DAL
{
    public enum RequestStatus
    {
        New,
        WaitingForApproval,
        InProgress,
        Completed,
        Cancelled,
        AwaitingPickUp,
        PickedUp
    }
}
