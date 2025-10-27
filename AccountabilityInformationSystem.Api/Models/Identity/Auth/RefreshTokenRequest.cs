namespace AccountabilityInformationSystem.Api.Models.Identity.Auth;

public sealed record RefreshTokenRequest
{
    public required string RefreshToken { get; init; }
}
