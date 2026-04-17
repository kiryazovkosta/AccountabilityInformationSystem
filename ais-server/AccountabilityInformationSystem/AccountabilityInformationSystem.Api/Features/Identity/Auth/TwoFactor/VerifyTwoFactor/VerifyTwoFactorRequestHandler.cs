using System.Security.Cryptography;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.SetupTwoFactor;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.VerifyTwoFactor;

public sealed class VerifyTwoFactorRequestHandler(
    UserManager<IdentityUser> userManager,
    IDataProtectionProvider dataProtectionProvider)
{
    private readonly ITimeLimitedDataProtector _setupProtector =
        dataProtectionProvider.CreateProtector("TwoFactorSetupToken").ToTimeLimitedDataProtector();

    public async Task<Result<VerifyTwoFactorResponse>> Handle(VerifyTwoFactorRequest request)
    {
        string userId = string.Empty;
        try
        {
            userId = _setupProtector.Unprotect(request.SetupToken);
        }
        catch(CryptographicException)
        {
            return Result<VerifyTwoFactorResponse>.Failure(
                new Error("SetupToken", "Invalid or expired setup token."));
        }

        IdentityUser? identityUser = await userManager.FindByIdAsync(userId);
        if (identityUser is null)
        {
            return Result<VerifyTwoFactorResponse>.Failure(
                new Error("UserId", "User not found."),
                ResultFailureType.NotFound);
        }

        bool isValid = await userManager.VerifyTwoFactorTokenAsync(
            identityUser,
            userManager.Options.Tokens.AuthenticatorTokenProvider,
            request.Code);

        if (!isValid)
        {
            return Result<VerifyTwoFactorResponse>.Failure(
                new Error("SetupToken", "Invalid authenticator code!"));
        }

        await userManager.SetTwoFactorEnabledAsync(identityUser, true);
        IEnumerable<string>? recoveryCodes =
            await userManager.GenerateNewTwoFactorRecoveryCodesAsync(identityUser, 10);
        if (recoveryCodes is null || !recoveryCodes.Any())
        {
            return Result<VerifyTwoFactorResponse>.Failure(
                new Error("SetupToken", "Failed to generate new TwoFactor recovery codes!"));
        }

        return Result<VerifyTwoFactorResponse>.Success(new VerifyTwoFactorResponse() { RecoveryCodes = recoveryCodes });
    }
}
