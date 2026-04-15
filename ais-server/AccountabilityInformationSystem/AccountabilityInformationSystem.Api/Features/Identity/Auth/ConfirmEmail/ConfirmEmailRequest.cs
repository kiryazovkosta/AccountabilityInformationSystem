namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.ConfirmEmail;

public sealed record ConfirmEmailRequest(string UserId, string Code);
