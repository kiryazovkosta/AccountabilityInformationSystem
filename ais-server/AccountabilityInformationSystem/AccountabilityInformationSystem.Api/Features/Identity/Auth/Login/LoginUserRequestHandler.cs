using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Settings;
using AccountabilityInformationSystem.Api.Shared.Services.Tokenizing;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;

public class LoginUserRequestHandler(
    ApplicationDbContext applicationDbContext,
    ApplicationIdentityDbContext identityDbContext,
    UserManager<IdentityUser> userManager,
    TokenProvider tokenProvider,
    IOptions<JwtAuthOptions> options,
    IDataProtectionProvider dataProtectionProvider)
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;
    private readonly ITimeLimitedDataProtector _setupProtector =
        dataProtectionProvider.CreateProtector("TwoFactorSetupToken").ToTimeLimitedDataProtector();


    public async Task<Result<LoginUserResponse>> Handle(LoginUserRequest request, CancellationToken cancellationToken)
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
            User? appUser = await applicationDbContext.Users
                .FirstOrDefaultAsync(u => u.IdentityId == identityUser.Id, cancellationToken);

            if (appUser?.Enable2Fa == true)
            {
                if (!identityUser.EmailConfirmed)
                {
                    return Result<LoginUserResponse>.Failure(
                        new Error("ErrorCodes.Forbidden", "Email must be confirmed before setting up two-factor authentication."),
                        ResultFailureType.Forbidden);
                }

                string setupToken = _setupProtector.Protect(identityUser.Id, TimeSpan.FromMinutes(10));
                return Result<LoginUserResponse>.Success(
                    new LoginUserResponse(RequiresTwoFactorSetup: true, SetupToken: setupToken),
                    ResultSuccessType.Accepted);
            }
        }

        // Handle 2FA validation when enabled — code must be provided in the same request
        if (identityUser.TwoFactorEnabled)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return Result<LoginUserResponse>.Failure(
                    new Error("ErrorCodes.Unauthorized", "Authenticator code is required!"),
                    ResultFailureType.Unauthorized);
            }

            bool isValid = await userManager.VerifyTwoFactorTokenAsync(
                identityUser,
                userManager.Options.Tokens.AuthenticatorTokenProvider,
                request.Code);

            if (!isValid)
            {
                return Result<LoginUserResponse>.Failure(
                    new Error("ErrorCodes.Unauthorized", "Invalid authenticator code!"),
                    ResultFailureType.Unauthorized);
            }
        }

        IEnumerable<string> roles = await userManager.GetRolesAsync(identityUser);
        AccessTokenRequest accessTokenRequest = new(identityUser.Id, identityUser.UserName ?? string.Empty, roles);
        AccessTokenResponse response = tokenProvider.Create(accessTokenRequest);
        RefreshToken refreshToken = new()
        {
            Id = $"rt_{Guid.CreateVersion7()}",
            UserId = identityUser.Id,
            Token = response.RefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays)
        };

        await identityDbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await identityDbContext.SaveChangesAsync(cancellationToken);

        return Result<LoginUserResponse>.Success(
            new LoginUserResponse(AccessToken: response.AccessToken, RefreshToken: response.RefreshToken));
    }
}
