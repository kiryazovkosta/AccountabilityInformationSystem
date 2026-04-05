namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor;
public record class SetupTwoFactorRequest
{
    public required string SetupToken { get; init; }
}