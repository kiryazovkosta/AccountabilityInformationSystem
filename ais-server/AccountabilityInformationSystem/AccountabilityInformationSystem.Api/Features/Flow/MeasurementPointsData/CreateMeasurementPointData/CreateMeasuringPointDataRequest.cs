using AccountabilityInformationSystem.Api.Domain.Entities.Flow;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.CreateMeasurementPointData;

public sealed record CreateMeasuringPointDataRequest
{
    public string MeasurementPointId { get; init; }
    public int Number { get; init; }
    public DateTime BeginTime { get; init; }
    public DateTime EndTime { get; init; }
    public FlowDirectionType FlowDirectionType { get; init; }
    public string ProductId { get; init; }
    public decimal? TotalizerBeginGrossObservableVolume { get; init; }
    public decimal? TotalizerEndGrossObservableVolume { get; init; }
    public decimal? TotalizerBeginGrossStandardVolume { get; init; }
    public decimal? TotalizerEndGrossStandardVolume { get; init; }
    public decimal? TotalizerBeginMass { get; init; }
    public decimal? TotalizerEndMass { get; init; }
    public decimal? GrossObservableVolume { get; init; }
    public decimal? GrossStandardVolume { get; init; }
    public decimal? Mass { get; init; }
    public decimal? AverageObservableDensity { get; init; }
    public decimal? AverageReferenceDensity { get; init; }
    public decimal? AverageTemperature { get; init; }
    public decimal? AlcoholContent { get; init; }
    public string? BatchNumber { get; init; }
    public string? ExternalId { get; init; }
}
