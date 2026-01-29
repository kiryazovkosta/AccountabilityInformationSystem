using AccountabilityInformationSystem.Api.Domain.Entities.Flow;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.CreateMeasurementPointData;

public sealed record CreateMeasuringPointDataRequest
{
    public required string MeasurementPointId { get; init; }
    public required int Number { get; init; }
    public required DateTime BeginTime { get; init; }
    public required DateTime EndTime { get; init; }
    public required FlowDirectionType FlowDirectionType { get; init; }
    public required string ProductId { get; init; }
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
