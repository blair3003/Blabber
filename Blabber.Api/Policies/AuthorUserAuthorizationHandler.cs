using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Blabber.Api.Policies
{
    public class AuthorUserAuthorizationHandler : AuthorizationHandler<AuthorUserRequirement, string>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AuthorUserRequirement requirement,
            string applicationUserId)
        {
            var currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == applicationUserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}