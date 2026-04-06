using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepairWorkShop.DAL
{
    internal class AppDbContext : DbContext
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
    }
}
