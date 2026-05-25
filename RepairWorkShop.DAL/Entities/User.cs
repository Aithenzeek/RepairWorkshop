namespace RepairWorkShop.DAL.Entities
{
    public class User : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int RoleId { get; set; }

        public UserRole Role { get; set; } = null!;
    }
}
