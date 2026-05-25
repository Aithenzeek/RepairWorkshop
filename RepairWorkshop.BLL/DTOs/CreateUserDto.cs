namespace RepairWorkshop.BLL.DTOs
{
    public record CreateUserDto(
        string Name,
        string Phone,
        int RoleId
    );
}
