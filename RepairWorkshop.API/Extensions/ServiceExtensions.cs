using RepairWorkshop.BLL.Interfaces;
using RepairWorkshop.BLL.Services;

namespace RepairWorkshop.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICustomerRequestService, CustomerRequestService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IRepairItemService, RepairItemService>();
            services.AddScoped<IServiceTaskService, ServiceTaskService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRolePermissionService, RolePermissionService>();

            return services;
        }

    }
}
