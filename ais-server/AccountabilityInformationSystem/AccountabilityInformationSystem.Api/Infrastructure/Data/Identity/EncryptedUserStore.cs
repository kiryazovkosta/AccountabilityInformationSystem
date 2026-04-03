using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Constants;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Infrastructure.Data.Identity;

public class EncryptedUserStore<TUser> : UserStore<TUser>
    where TUser : IdentityUser, new()
{
    private readonly IDataProtector protector;
    public EncryptedUserStore(ApplicationIdentityDbContext context, IDataProtectionProvider provider)
        : base(context)
    {
        protector = provider.CreateProtector("TwoFactorSecret");
    }

    public override Task SetTokenAsync(TUser user, string loginProvider, string name,
        string? value, CancellationToken cancellationToken)
    {
        if (name == ApplicationConstants.AuthenticatorTokenKey && value is not null)
        {
            value = protector.Protect(value);
        }

        return base.SetTokenAsync(user, loginProvider, name, value, cancellationToken);
    }

    public override async Task<string?> GetTokenAsync(TUser user, string loginProvider, string name,
        CancellationToken cancellationToken)
    {
        string? protectedValue = await base.GetTokenAsync(user, loginProvider, name, cancellationToken);
        if (name == ApplicationConstants.AuthenticatorTokenKey && protectedValue is not null)
        {
            return protector.Unprotect(protectedValue);
        }

        return protectedValue;
    }
}
