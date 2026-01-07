using AccountabilityInformationSystem.Api.Entities.Abstraction;

namespace AccountabilityInformationSystem.Api.Entities.Flow;

public sealed class Ikunk
    : AuditableEntity, IEntity, IActivableEntity, IOrderPositionEntity 
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string FullName { get; set; }
    public string? Description { get; set; }
    public int OrderPosition { get; set; }
    public DateOnly ActiveFrom { get; set; }
    public DateOnly ActiveTo { get; set; }
    public string WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }
    public ICollection<MeasurementPoint> MeasurementPoints { get; set; } = new List<MeasurementPoint>();
}
