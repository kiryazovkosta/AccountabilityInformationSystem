namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.ForgotPassword;

public sealed record ForgotPasswordRequest
{
    public string Username { get; init; }
}
