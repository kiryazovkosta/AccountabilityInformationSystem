using AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;
using AccountabilityInformationSystem.Api.Shared;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth;

internal sealed class AuthModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddTransient<TokenProvider>();
        services.AddSingleton<TwoFactorSetupTokenProtector>();
        services.AddTransient<EmailConfirmationService>();
        services.AddTransient<PasswordResetEmailService>();
    }
}
