namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;

public sealed record LoginUserRequest
{
    public required string Username { get; init; }
    public required string Password { get; init; }
    public string? Code { get; init; }
}
