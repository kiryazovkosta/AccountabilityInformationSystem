namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.VerifyTwoFactor;

public sealed record VerifyTwoFactorRequest
{
    public required string SetupToken { get; init; }
    public required string Code { get; init; }
}