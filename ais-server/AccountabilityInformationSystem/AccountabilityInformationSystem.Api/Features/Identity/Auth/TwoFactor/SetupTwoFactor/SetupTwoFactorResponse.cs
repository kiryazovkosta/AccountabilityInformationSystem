namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.SetupTwoFactor;

public sealed record SetupTwoFactorResponse(string QrCodeBase64, string ManualEntryKey);