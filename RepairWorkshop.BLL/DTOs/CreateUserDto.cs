using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.DTOs
{
    public record CreateUserDto(
        string Name,
        string Phone,
        UserRole Role
    );
}
