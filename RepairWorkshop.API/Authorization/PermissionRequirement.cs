using Microsoft.AspNetCore.Authorization;

namespace RepairWorkshop.API.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public List<string> Permissions { get; }

        public PermissionRequirement(IEnumerable<string> permissions)
        {
            Permissions = permissions.ToList();
        }
    }
}
