namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor;

public sealed record SetupTwoFactorResponse(string QrCodeBase64, string ManualEntryKey);