using AccountabilityInformationSystem.Api.Entities.Flow;
using AccountabilityInformationSystem.Api.Models.Common;
using AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;
using AccountabilityInformationSystem.Api.Models.Products;

namespace AccountabilityInformationSystem.Api.Models.Flow.MeasurementPointsData;

public sealed record MeasurementPointDataListResponse : ILinksResponse
{
    public string Id { get; init; }
    public MeasurementPointListResponse MeasurementPoint { get; init; }
    public int Number { get; init; }
    public DateTime BeginTime { get; init; }
    public DateTime EndTime { get; init; }
    public EnumTypeResponse FlowDirection { get; init; }
    public ProductListResponse Product { get; init; }
    public decimal? GrossObservableVolume { get; init; }
    public decimal? GrossStandardVolume { get; init; }
    public decimal? Mass { get; init; }
    public List<LinkResponse> Links { get; set; }
}
