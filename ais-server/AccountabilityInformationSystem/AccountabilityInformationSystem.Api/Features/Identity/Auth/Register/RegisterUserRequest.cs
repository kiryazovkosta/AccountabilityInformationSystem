using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;

public sealed record RegisterUserRequest
{
    public string Username { get; init; }
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string LastName { get; init; }
    public string? Image { get; init; }
    public string Password { get; init; }
    public string ConfirmPassword { get; init; }
    public required bool Enable2Fa { get; init; }
}