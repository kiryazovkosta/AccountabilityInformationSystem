namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.ResetPassword;

public sealed record ResetPasswordRequest
{
    public string UserId { get; init; }
    public string Code { get; init; }
    public string NewPassword { get; init; }
    public string ConfirmPassword { get; init; }
}
