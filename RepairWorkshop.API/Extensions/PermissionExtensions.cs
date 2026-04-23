using Microsoft.AspNetCore.Authorization;
using RepairWorkshop.API.Authorization;

namespace RepairWorkshop.API.Extensions
{
    public static class PermissionExtensions
    {
        public static IServiceCollection AddPermission(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

            return services;
        }
    }
}
