using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.DTOs
{
    public record EditUserDto(
        string Name,
        string Phone,
        UserRole Role
    );
}
