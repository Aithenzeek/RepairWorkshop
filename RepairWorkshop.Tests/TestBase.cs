using Microsoft.EntityFrameworkCore;
using RepairWorkShop.DAL;

namespace RepairWorkshop.Tests
{
    public class TestBase
    {
        protected AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            var context = new AppDbContext(options);

            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            return context;
        }
    }
}
