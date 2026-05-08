using RepairWorkShop.DAL.Enums;

namespace RepairWorkshop.BLL.DTOs
{
    public record ResponseRepairItemDto(
        int Id,
        int CustomerRequestId,
        string? Model,
        string? SerialNumber,
        string? ProblemDescription,
        string? Notes,
        RepairItemStatus Status,
        double? ServiceCost,
        DateTime? StartedAt,
        DateTime? CompletedAt
    );
}
