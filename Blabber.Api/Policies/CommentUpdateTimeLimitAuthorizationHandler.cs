using Microsoft.AspNetCore.Authorization;

namespace Blabber.Api.Policies
{
    public class CommentUpdateTimeLimitAuthorizationHandler : AuthorizationHandler<CommentUpdateTimeLimitRequirement, DateTime>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CommentUpdateTimeLimitRequirement requirement,
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