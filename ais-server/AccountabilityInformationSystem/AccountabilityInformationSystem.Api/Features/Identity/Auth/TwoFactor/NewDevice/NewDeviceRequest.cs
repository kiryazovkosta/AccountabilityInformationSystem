namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.NewDevice;

public sealed record NewDeviceRequest
{
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string RecoveryCode { get; init; }
}
