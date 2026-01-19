using AccountabilityInformationSystem.Api.Shared.Models;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;

public sealed record MeasurementPointDataListResponse : ILinksResponse
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
}
