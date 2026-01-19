namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;

public sealed record AccessTokenRequest(string UserId, string Email, IEnumerable<string> Roles);
