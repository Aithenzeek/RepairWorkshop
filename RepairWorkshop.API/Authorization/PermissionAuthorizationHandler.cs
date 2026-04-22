    using Microsoft.AspNetCore.Authorization;

namespace RepairWorkshop.API.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
        {
            foreach (var c in context.User.Claims)
            {
                Console.WriteLine($"{c.Type} = {c.Value}");
            }

            var userPermissions = context.User.Claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value);

            if (requirement.Permissions.Any(p => userPermissions.Contains(p)))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
