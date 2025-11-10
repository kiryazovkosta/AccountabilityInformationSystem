namespace AccountabilityInformationSystem.Api.Models.Identity.Auth;

public sealed record AccessTokenRequest(string UserId, string Email, IEnumerable<string> Roles);
