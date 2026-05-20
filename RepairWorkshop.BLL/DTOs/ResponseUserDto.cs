namespace RepairWorkshop.BLL.DTOs
{
    public record ResponseUserDto(
        int Id,
        string Name,
        string Phone,
        int RoleId,
        string RoleName
    );
}
