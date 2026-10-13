namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;

public sealed record LoginUserResponse(
    bool RequiresTwoFactorSetup = false,
    string? SetupToken = null,
    string? AccessToken = null,
    string? RefreshToken = null);
