namespace RepairWorkshop.BLL.DTOs
{
    public record CreateServiceTaskDto(
        int RepairItemId,
        int WorkerId, 
        int ServiceId
    );
}
