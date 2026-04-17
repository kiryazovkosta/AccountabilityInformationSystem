namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.VerifyTwoFactor;

public sealed record VerifyTwoFactorResponse
{
    public IEnumerable<string> RecoveryCodes { get; init; }
}
