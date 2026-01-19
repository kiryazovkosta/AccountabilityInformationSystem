using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;

namespace AccountabilityInformationSystem.Api.Domain.Entities.Identity;

public sealed class User : AuditableEntity, IEntity
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string? Image { get; set; }
    public string? IdentityId { get; set; }
}
