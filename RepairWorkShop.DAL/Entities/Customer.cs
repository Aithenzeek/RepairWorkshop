namespace RepairWorkShop.DAL.Entities
{
    public class Customer : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}