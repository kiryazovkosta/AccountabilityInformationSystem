using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Warehouses.Shared;

public sealed record WarehouseIkunkResponse : IMapFrom<Ikunk>, IMapCustom
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public List<WarehouseIkunkMeasurementPointResponse> MeasurementPoints { get; init; }

    public void CreateMappings(Mapster.TypeAdapterConfig config) =>
        config.NewConfig<Ikunk, WarehouseIkunkResponse>()
            .Map(dest => dest.MeasurementPoints, src => src.MeasurementPoints.OrderBy(mp => mp.OrderPosition));
}
