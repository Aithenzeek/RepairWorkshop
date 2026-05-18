using RepairWorkShop.DAL.Enums;

namespace RepairWorkshop.BLL.DTOs
{
    public record ResponseServiceDto(
        int Id,
        string Name,
        double Price,
        ServiceStatus Status
    );
}
