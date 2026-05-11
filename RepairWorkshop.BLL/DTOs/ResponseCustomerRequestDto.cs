using RepairWorkShop.DAL.Enums;

namespace RepairWorkshop.BLL.DTOs
{
    public record ResponseCustomerRequestDto(
        int Id,
        int? CustomerId,
        int? ManagerId,
        DateTime? StartedAt,
        RequestStatus Status,
        DateTime? CompletedAt,
        double TotalCost
    );
}
