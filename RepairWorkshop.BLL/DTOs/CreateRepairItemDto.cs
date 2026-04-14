namespace RepairWorkshop.BLL.DTOs
{
    public record CreateRepairItemDto(
        string Model,
        string SerialNumber,
        string ProblemDescription,
        string Notes
    );
}
