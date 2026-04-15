namespace RepairWorkshop.BLL.DTOs
{
    public record EditRepairItemDto(
        string Model,
        string SerialNumber,
        string ProblemDescription,
        string Notes
    );
}
