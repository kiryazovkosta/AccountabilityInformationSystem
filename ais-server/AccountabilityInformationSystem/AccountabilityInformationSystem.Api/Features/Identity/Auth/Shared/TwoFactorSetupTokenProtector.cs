using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;

public sealed class TwoFactorSetupTokenProtector(IDataProtectionProvider dataProtectionProvider)
{
    private const string Purpose = "TwoFactorSetupToken";
    private static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(10);

    private readonly ITimeLimitedDataProtector _protector =
        dataProtectionProvider.CreateProtector(Purpose).ToTimeLimitedDataProtector();

    public string Protect(string userId) => _protector.Protect(userId, Lifetime);

    public bool TryUnprotect(string setupToken, [NotNullWhen(true)] out string? userId)
    {
        try
        {
            userId = _protector.Unprotect(setupToken);
            return true;
        }
        catch (CryptographicException)
        {
            userId = null;
            return false;
        }
    }
}
