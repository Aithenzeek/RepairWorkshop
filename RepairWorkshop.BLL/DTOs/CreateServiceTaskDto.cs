namespace RepairWorkshop.BLL.DTOs
{
    public record CreateServiceTaskDto(
        int RepairItemId,
        int UserId, 
        int ServiceId
    );
}
