using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.NewDevice;

public sealed class NewDeviceRequestHandler(
    UserManager<IdentityUser> userManager,
    IDataProtectionProvider dataProtectionProvider)
{
    private readonly ITimeLimitedDataProtector _setupProtector =
        dataProtectionProvider.CreateProtector("TwoFactorSetupToken").ToTimeLimitedDataProtector();

    public async Task<Result<LoginUserResponse>> Handle(NewDeviceRequest request)
    {
        IdentityUser? identityUser = await userManager.FindByNameAsync(request.Username);
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

        IdentityResult redeemResult = await userManager.RedeemTwoFactorRecoveryCodeAsync(identityUser, request.RecoveryCode);
        if (!redeemResult.Succeeded)
        {
            return Result<LoginUserResponse>.Failure(
                new Error("ErrorCodes.Unauthorized", "Invalid or already-used recovery code."),
                ResultFailureType.Unauthorized);
        }

        await userManager.SetTwoFactorEnabledAsync(identityUser, false);

        string setupToken = _setupProtector.Protect(identityUser.Id, TimeSpan.FromMinutes(10));
        return Result<LoginUserResponse>.Success(
            new LoginUserResponse(RequiresTwoFactorSetup: true, SetupToken: setupToken),
            ResultSuccessType.Accepted);
    }
}
