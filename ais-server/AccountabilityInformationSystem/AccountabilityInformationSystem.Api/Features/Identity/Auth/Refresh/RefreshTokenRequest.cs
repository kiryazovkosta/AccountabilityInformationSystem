namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.Refresh;

public sealed record RefreshTokenRequest
{
    public required string RefreshToken { get; init; }
}
