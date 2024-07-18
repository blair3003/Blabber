using Microsoft.AspNetCore.Authorization;

namespace Blabber.Api.Policies
{
    public class BlabUpdateTimeLimitRequirement(int maxUpdateWindowMinutes = 5) : IAuthorizationRequirement
    {
        public int MaxUpdateWindowMinutes { get; } = maxUpdateWindowMinutes;
    }
}