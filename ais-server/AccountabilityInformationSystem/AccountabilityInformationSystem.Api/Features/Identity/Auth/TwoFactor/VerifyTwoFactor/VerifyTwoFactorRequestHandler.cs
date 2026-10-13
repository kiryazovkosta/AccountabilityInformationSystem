using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;
using Microsoft.AspNetCore.Identity;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.VerifyTwoFactor;

public sealed class VerifyTwoFactorRequestHandler(
    UserManager<IdentityUser> userManager,
    TwoFactorSetupTokenProtector setupTokenProtector)
{
    public async Task<Result<VerifyTwoFactorResponse>> Handle(VerifyTwoFactorRequest request, CancellationToken cancellationToken)
    {
        _ = cancellationToken;

        if (!setupTokenProtector.TryUnprotect(request.SetupToken, out string? userId))
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
