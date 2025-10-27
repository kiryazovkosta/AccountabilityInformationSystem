using AccountabilityInformationSystem.Api.Entities.Abstraction;
using Microsoft.AspNetCore.Identity;

namespace AccountabilityInformationSystem.Api.Entities.Identity;

public sealed class RefreshToken : IEntity
{
    public string Id { get; set; }
    public required string UserId { get; set; }
    public required string Token { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public IdentityUser User { get; set; }
}
