using Microsoft.EntityFrameworkCore;
using RepairWorkShop.DAL.Entities;
using RepairWorkShop.DAL.Interfaces;

namespace RepairWorkShop.DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<CustomerRequest> Requests { get; set; }
        public DbSet<RepairItem> RepairItems { get; set; }
        public DbSet<ServiceTask> ServiceTasks { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public string DbPath { get; }
        private readonly ICurrentUserService? _currentUser;

        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        {
            //var folder = Environment.SpecialFolder.LocalApplicationData;
            //var path = Environment.GetFolderPath(folder);

            //DbPath = System.IO.Path.Join(path, "app.db");
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{ 
        //    optionsBuilder.UseSqlite($"Data Source={DbPath}");
        //}

        //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    var entries = ChangeTracker
        //        .Entries<AuditableEntity>();

        //    var now = DateTime.Now;
        //    var userId = _currentUser?.UserId ?? "system";

        //    foreach (var entry in entries)
        //    {
        //        if (entry.State == EntityState.Added)
        //        {
        //            entry.Entity.CreatedAt = now;
        //            entry.Entity.CreatedBy = userId;
        //        }

        //        if (entry.State == EntityState.Modified)
        //        {
        //            entry.Entity.UpdatedAt = now;
        //            entry.Entity.UpdatedBy = userId;
        //        }
        //    }

        //    return await base.SaveChangesAsync(cancellationToken);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerRequest>()
                .HasMany(e => e.RepairItems)
                .WithOne(e => e.CustomerRequest)
                .HasForeignKey(e => e.CustomerRequestId)
                .IsRequired();

            modelBuilder.Entity<RepairItem>()
                .HasMany(e => e.ServiceTasks)
                .WithOne(e => e.RepairItem)
                .HasForeignKey(e => e.RepairItemId)
                .IsRequired();

            modelBuilder.Entity<ServiceTask>()
                .HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServiceId)
                .IsRequired();

            modelBuilder.Entity<CustomerRequest>()
                .HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .IsRequired(false);

            modelBuilder.Entity<ServiceTask>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasOne(e => e.Role)
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .IsRequired();

            modelBuilder.Entity<RolePermission>()
                .HasKey(e => new { e.UserRoleId, e.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(e => e.UserRole)
                .WithMany(e => e.RolePermissions)
                .HasForeignKey(e => e.UserRoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId);

            modelBuilder.Entity<Permission>()
                    .HasIndex(p => p.Code)
                    .IsUnique();

            modelBuilder.Entity<CustomerRequest>()
                    .Property(e => e.Status)
                    .HasConversion<string>();

            modelBuilder.Entity<RepairItem>()
                    .Property(e => e.Status)
                    .HasConversion<string>();

            modelBuilder.Entity<ServiceTask>()
                    .Property(e => e.Status)
                    .HasConversion<string>();

            modelBuilder.Entity<Service>()
                    .Property(e => e.Status)
                    .HasConversion<string>();
        }
    }
}
