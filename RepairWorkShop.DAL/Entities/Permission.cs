namespace RepairWorkShop.DAL.Entities
{
    public class Permission : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
