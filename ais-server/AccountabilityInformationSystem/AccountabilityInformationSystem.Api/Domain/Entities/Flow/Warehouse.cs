using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;

namespace AccountabilityInformationSystem.Api.Domain.Entities.Flow;

public sealed class Warehouse
    : AuditableEntity, IEntity, IActivableEntity, IOrderPositionEntity
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string FullName { get; set; }
    public string? Description { get; set; }
    public int OrderPosition { get; set; }
    public string ExciseNumber { get; set; }
    public DateOnly ActiveFrom { get; set; }
    public DateOnly ActiveTo { get; set; }
    public ICollection<Ikunk> Ikunks { get; set; } = [];
}
