using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;

public sealed record IkunkResponse : IMapFrom<Ikunk>, IMapCustom
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string FullName { get; init; }
    public string? Description { get; init; }
    public int OrderPosition { get; init; }
    public DateOnly ActiveFrom { get; init; }
    public DateOnly ActiveTo { get; init; }
    public IkunkWarehouseResponse? Warehouse { get; init; }
    public List<IkunkMeasurementPointResponse> MeasurementPoints { get; init; }

    public void CreateMappings(Mapster.TypeAdapterConfig config)
    {
        config.NewConfig<Ikunk, IkunkResponse>()
            .Map(dest => dest.MeasurementPoints, src => src.MeasurementPoints.OrderBy(p => p.OrderPosition));
    }
}
