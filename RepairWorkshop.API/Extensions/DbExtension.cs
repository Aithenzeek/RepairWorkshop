using Microsoft.EntityFrameworkCore;
using RepairWorkShop.DAL;

namespace RepairWorkshop.API.Extensions
{
    public static class DbExtension
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=app.db"));

            return services;
        }
    }
}
