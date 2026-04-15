using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Settings;
using AccountabilityInformationSystem.Api.Shared.Services.Tokenizing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.Refresh;

public class RefreshTokenRequestHandler(
    UserManager<IdentityUser> userManager,
    ApplicationIdentityDbContext identityDbContext,
    TokenProvider tokenProvider,
    IOptions<JwtAuthOptions> options
    )
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    public async Task<Result<AccessTokenResponse>> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        RefreshToken? storedToken = await identityDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
        {
            return Result<AccessTokenResponse>.Failure(
                new Error("Unauthorized", "Token expired or invalid."),
                ResultFailureType.Unauthorized);
        }

        IList<string> roles = await userManager.GetRolesAsync(storedToken.User);
        AccessTokenRequest accessTokenRequest = new(
            storedToken.User.Id,
            storedToken.User.Email ?? string.Empty,
            roles);

        AccessTokenResponse response = tokenProvider.Create(accessTokenRequest);

        storedToken.Token = response.RefreshToken;
        storedToken.ExpiresAt = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays);
        identityDbContext.RefreshTokens.Update(storedToken);
        await identityDbContext.SaveChangesAsync(cancellationToken);

        return Result<AccessTokenResponse>.Success(response);
    }
}
