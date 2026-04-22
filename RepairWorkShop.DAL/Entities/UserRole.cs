namespace RepairWorkShop.DAL.Entities
{
    public class UserRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<RolePermission> RolePermissions { get; set; } = [];
    }
}
