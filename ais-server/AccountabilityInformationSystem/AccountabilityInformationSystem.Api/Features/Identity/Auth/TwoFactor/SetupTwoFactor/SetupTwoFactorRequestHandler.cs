using System.Security.Cryptography;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.VerifyTwoFactor;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using QRCoder;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.SetupTwoFactor;

public sealed class SetupTwoFactorRequestHandler(
    UserManager<IdentityUser> userManager,
    IDataProtectionProvider dataProtectionProvider)
{
    private readonly ITimeLimitedDataProtector _setupProtector =
        dataProtectionProvider.CreateProtector("TwoFactorSetupToken").ToTimeLimitedDataProtector();

    public async Task<Result<SetupTwoFactorResponse>> Handle(SetupTwoFactorRequest request)
    {
        string userId = string.Empty;
        try
        {
            userId = _setupProtector.Unprotect(request.SetupToken);
        }
        catch(CryptographicException)
        {
            return Result<SetupTwoFactorResponse>.Failure(
                new Error("SetupToken", "Invalid or expired setup token."));
        }

        IdentityUser? identityUser = await userManager.FindByIdAsync(userId);
        if (identityUser is null)
        {
            return Result<SetupTwoFactorResponse>.Failure(
                new Error("UserId", "User not found."), 
                ResultFailureType.NotFound); 
        }

        if (identityUser.TwoFactorEnabled)
        {
            return Result<SetupTwoFactorResponse>.Failure(
                new Error("TwoFactorEnabled", "Two-factor authentication is already configured."),
                ResultFailureType.Conflict);
        }

        await userManager.ResetAuthenticatorKeyAsync(identityUser);
        string? key = await userManager.GetAuthenticatorKeyAsync(identityUser);
        if (string.IsNullOrEmpty(key))
        {
            return Result<SetupTwoFactorResponse>.Failure(
                new Error("Key", "Failed to return authentication key for the user"));
        }

        string otpauthUri =
            $"otpauth://totp/AIS:{identityUser.Email}?secret={key}&issuer=AIS&digits=6&algorithm=SHA1&period=30";

        using QRCodeGenerator qrGenerator = new();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(otpauthUri, QRCodeGenerator.ECCLevel.Q);
        using PngByteQRCode qrCode = new(qrCodeData);
        byte[] qrCodeBytes = qrCode.GetGraphic(20);
        string qrCodeBase64 = $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";

        return Result< SetupTwoFactorResponse>.Success(new SetupTwoFactorResponse(qrCodeBase64, key!));
    }
}
