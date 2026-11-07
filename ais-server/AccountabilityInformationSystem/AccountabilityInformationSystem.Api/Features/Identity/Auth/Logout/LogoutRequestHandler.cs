using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.Tokenizing;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.Logout;

public sealed class LogoutRequestHandler(ApplicationIdentityDbContext identityDbContext)
{
    // Logout is best-effort: an unknown or expired refresh token means there is nothing
    // left to revoke, so it still succeeds and the caller clears the auth cookies.
    public async Task<Result> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshTokenValue))
        {
            return Result.Success();
        }

        string tokenHash = RefreshTokenHasher.Hash(request.RefreshTokenValue);
        var refreshToken = await identityDbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == tokenHash, cancellationToken);

        if (refreshToken is not null)
        {
            identityDbContext.RefreshTokens.Remove(refreshToken);
            await identityDbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
