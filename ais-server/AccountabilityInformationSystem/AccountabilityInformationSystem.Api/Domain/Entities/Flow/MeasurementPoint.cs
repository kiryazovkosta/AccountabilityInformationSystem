using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;

namespace AccountabilityInformationSystem.Api.Domain.Entities.Flow;

public sealed class MeasurementPoint
    : AuditableEntity, IEntity, IActivableEntity, IOrderPositionEntity
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string FullName { get; set; }
    public string? Description { get; set; }
    public string ControlPoint { get; set; }
    public int OrderPosition { get; set; }
    public FlowDirectionType FlowDirection { get; set; }
    public TransportType Transport { get; set; }
    public DateOnly ActiveFrom { get; set; }
    public DateOnly ActiveTo { get ; set; }
    public string IkunkId { get; set; }
    public Ikunk Ikunk { get; set; }
}
