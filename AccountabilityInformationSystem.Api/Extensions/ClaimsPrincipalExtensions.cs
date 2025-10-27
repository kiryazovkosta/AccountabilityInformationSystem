using System.Security.Claims;

namespace AccountabilityInformationSystem.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetIdentityId(this ClaimsPrincipal? claimsPrincipal)
    {
        return claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
