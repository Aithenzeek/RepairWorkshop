using Microsoft.AspNetCore.Authorization;

namespace RepairWorkshop.API.Authorization
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(params string[] permissions)
        {
            Policy = string.Join(",", permissions);
        }
    }
}
