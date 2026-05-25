using Microsoft.EntityFrameworkCore;
using RepairWorkShop.DAL;
using RepairWorkShop.DAL.Interceptors;

namespace RepairWorkshop.API.Extensions
{
    public static class DbExtension
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "app.db");

                options.UseSqlite($"Data Source={path}");

                options.AddInterceptors(
                    sp.GetRequiredService<AuditableInterceptor>());
            });

            return services;
        }
    }
}
