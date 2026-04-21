namespace RepairWorkShop.DAL.Entities
{
    public class RolePermission
    {
        public int UserRoleId { get; set; }
        public int PermissionId { get; set; }

        public UserRole UserRole { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }
}
