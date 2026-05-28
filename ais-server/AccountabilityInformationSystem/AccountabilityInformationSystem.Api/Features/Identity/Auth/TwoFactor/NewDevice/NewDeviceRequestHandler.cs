using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Settings;
using AccountabilityInformationSystem.Api.Shared.Services.Tokenizing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.NewDevice;

public sealed class NewDeviceRequestHandler(
    ApplicationIdentityDbContext identityDbContext,
    UserManager<IdentityUser> userManager,
    TokenProvider tokenProvider,
    IOptions<JwtAuthOptions> options,
    TimeProvider timeProvider)
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    public async Task<Result<LoginUserResponse>> Handle(NewDeviceRequest request, CancellationToken cancellationToken)
    {
        IdentityUser identityUser = await userManager.FindByNameAsync(request.Username);
        if (identityUser is null ||
            !await userManager.CheckPasswordAsync(identityUser, request.Password))
        {
            return Result<LoginUserResponse>.Failure(
                new Error("ErrorCodes.Unauthorized", "Invalid username or password!"),
                ResultFailureType.Unauthorized);
        }

        if (!identityUser.TwoFactorEnabled)
        {
            return Result<LoginUserResponse>.Failure(
                new Error("ErrorCodes.Unauthorized", "Two-factor authentication is not enabled for this account."),
                ResultFailureType.Unauthorized);
        }

        IEnumerable<string> roles = await userManager.GetRolesAsync(identityUser);
        AccessTokenRequest accessTokenRequest = new(identityUser.Id, identityUser.UserName ?? string.Empty, roles);
        AccessTokenResponse response = tokenProvider.Create(accessTokenRequest);
        RefreshToken refreshToken = new()
        {
            Id = $"rt_{Guid.CreateVersion7()}",
            UserId = identityUser.Id,
            Token = response.RefreshToken,
            ExpiresAt = timeProvider.GetUtcNow().UtcDateTime.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays)
        };

        // Stage the refresh token before redemption so Identity's internal SaveChangesAsync
        // persists both the code removal and the new token atomically.
        await identityDbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);

        IdentityResult redeemResult = await userManager.RedeemTwoFactorRecoveryCodeAsync(identityUser, request.RecoveryCode);
        if (!redeemResult.Succeeded)
        {
            return Result<LoginUserResponse>.Failure(
                new Error("ErrorCodes.Unauthorized", "Invalid or already-used recovery code."),
                ResultFailureType.Unauthorized);
        }

        return Result<LoginUserResponse>.Success(
            new LoginUserResponse(AccessToken: response.AccessToken, RefreshToken: response.RefreshToken));
    }
}
