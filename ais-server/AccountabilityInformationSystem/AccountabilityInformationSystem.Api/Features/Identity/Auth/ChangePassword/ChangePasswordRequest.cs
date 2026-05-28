namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.ChangePassword;

public sealed record ChangePasswordRequest(string OldPassword, string NewPassword, string ConfirmPassword, string? Code);
