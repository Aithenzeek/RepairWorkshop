using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepairWorkShop.DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<CustomerRequest> Requests { get; set; }
        public DbSet<RepairItem> RepairItems { get; set; }
        public DbSet<ServiceTask> ServiceTasks { get; set; }
        public DbSet<Service> Services { get; set; }
        public string DbPath { get; }

        public AppDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);

            DbPath = System.IO.Path.Join(path, "app.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }

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
        }
    }
}
