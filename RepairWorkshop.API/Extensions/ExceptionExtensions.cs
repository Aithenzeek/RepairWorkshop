using RepairWorkshop.API.Exceptions;

namespace RepairWorkshop.API.Extensions
{
    public static class ExceptionExtensions
    {
        public static IServiceCollection AddExceptions(this IServiceCollection services)
        {
            services.AddExceptionHandler<BadRequestExceptionHandler>();
            services.AddExceptionHandler<ConflictExceptionHandler>();
            services.AddExceptionHandler<NotFoundExceptionHandler>();
            services.AddExceptionHandler<CustomGlobalExceptionHandler>();

            return services;
        }
    }
}
