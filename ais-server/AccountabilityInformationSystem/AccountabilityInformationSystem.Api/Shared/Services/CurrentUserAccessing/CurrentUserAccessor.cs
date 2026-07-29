using System.Security.Claims;
using AccountabilityInformationSystem.Api.Shared.Extensions;

namespace AccountabilityInformationSystem.Api.Shared.Services.CurrentUserAccessing;

public sealed record CurrentUser(string? UserId, string? UserName);

public sealed class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
{
    public CurrentUser GetCurrentUser()
    {
        ClaimsPrincipal? user = httpContextAccessor.HttpContext?.User;

        return new CurrentUser(user.GetIdentityId(), user?.Identity?.Name);
    }
}
