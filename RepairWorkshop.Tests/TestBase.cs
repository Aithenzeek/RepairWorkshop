using Microsoft.EntityFrameworkCore;
using RepairWorkShop.DAL;

namespace RepairWorkshop.Tests
{
    public class TestBase
    {
        protected AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
    }
}
