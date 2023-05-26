using Microsoft.AspNetCore.Authorization;

namespace IttpTest.Web.Tools;

public class RoleHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
    {
        var isAdmin = context.User.Claims.FirstOrDefault(claim => claim.Type == "Role")?.Value;
        
        if (isAdmin == "True" && requirement.Role == "Admin")
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        context.Fail();
        return Task.CompletedTask;
    }
}