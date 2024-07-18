using Microsoft.AspNetCore.Authorization;

namespace Blabber.Api.Policies
{
    public class BlabUpdateTimeLimitAuthorizationHandler : AuthorizationHandler<BlabUpdateTimeLimitRequirement, DateTime>
    {

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            BlabUpdateTimeLimitRequirement requirement,
            DateTime createdAt)
        {
            var timeSinceCreation = DateTime.UtcNow - createdAt;

            if (timeSinceCreation.TotalMinutes <= requirement.MaxUpdateWindowMinutes)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
