using RepairWorkShop.DAL.Enums;

namespace RepairWorkshop.BLL.DTOs
{
    public record ResponseServiceTaskDto(
        int Id,
        int RepairItemId,
        int UserId,
        int ServiceId,
        double? Cost,
        ServiceTaskStatus Status,
        DateTime? StartedAt,
        DateTime? CompletedAt,
        string? DiagnosticsResult,
        string? UserName,
        string? ServiceName
    );
}
