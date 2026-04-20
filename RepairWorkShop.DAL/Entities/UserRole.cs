using RepairWorkShop.DAL.Enums;

namespace RepairWorkShop.DAL.Entities
{
    public class UserRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<Permission> Permissions { get; set; } = [];
    }
}
