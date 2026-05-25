using RepairWorkShop.DAL.Interfaces;
using System.Security.Claims;

namespace RepairWorkshop.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _http;

        public CurrentUserService(IHttpContextAccessor http)
        {
            _http = http;
        }

        public string? UserId =>
            _http.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;
    }
}
