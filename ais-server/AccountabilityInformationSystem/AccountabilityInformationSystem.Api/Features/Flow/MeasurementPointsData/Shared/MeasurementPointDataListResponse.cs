using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;

public sealed record MeasurementPointDataListResponse : ILinksResponse, IMapFrom<MeasurementPointData>, IMapCustom
{
    public string Id { get; init; }
    public MeasurementPointDataMeasurementPointResponse MeasurementPoint { get; init; }
    public int Number { get; init; }
    public DateTime BeginTime { get; init; }
    public DateTime EndTime { get; init; }
    public EnumTypeResponse FlowDirection { get; init; }
    public MeasurementPointDataProducResponse Product { get; init; }
    public decimal? GrossObservableVolume { get; init; }
    public decimal? GrossStandardVolume { get; init; }
    public decimal? Mass { get; init; }
    public List<LinkResponse> Links { get; set; }

    public void CreateMappings(Mapster.TypeAdapterConfig config) =>
        config.NewConfig<MeasurementPointData, MeasurementPointDataListResponse>()
            .Map(dest => dest.FlowDirection, src => new EnumTypeResponse
            {
                Value = src.FlowDirectionType,
                Description = src.FlowDirectionType.GetDescription()
            });
}
