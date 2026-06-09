using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;

namespace AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;

public sealed class WarrantyBrand : AuditableEntity, IEntity
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Logo { get; set; }
    public ICollection<WarrantyRecord> WarrantyRecords { get; set; } = [];
}
