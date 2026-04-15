namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.ConfirmEmail;

public sealed record ConfirmEmailResponse(bool RequiresTwoFactorSetup, string? SetupToken = null);
