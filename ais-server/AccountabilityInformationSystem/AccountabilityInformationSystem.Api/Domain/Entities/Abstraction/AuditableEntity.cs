namespace AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;

public abstract class AuditableEntity
{
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
