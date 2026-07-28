using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;
using Mapster;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Update;

public sealed record UpdateMeasurementPointRequest : IMapTo<MeasurementPoint>, IMapCustom
{
    internal string Id { get; init; }
    public string? Name { get; init; }
    public string? FullName { get; init; }
    public string? Description { get; init; }
    public string? ControlPoint { get; init; }
    public int? OrderPosition { get; init; }
    public FlowDirectionType? FlowDirection { get; init; }
    public TransportType? Transport { get; init; }
    public DateOnly? ActiveFrom { get; init; }
    public DateOnly? ActiveTo { get; init; }
    public string? IkunkId { get; init; }

    public void CreateMappings(TypeAdapterConfig config) =>
        config.NewConfig<UpdateMeasurementPointRequest, MeasurementPoint>()
            .IgnoreNullValues(true)
            .Ignore(dest => dest.Id);
}
