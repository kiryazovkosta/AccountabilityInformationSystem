namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.SetupTwoFactor;
public record class SetupTwoFactorRequest
{
    public required string SetupToken { get; init; }
}