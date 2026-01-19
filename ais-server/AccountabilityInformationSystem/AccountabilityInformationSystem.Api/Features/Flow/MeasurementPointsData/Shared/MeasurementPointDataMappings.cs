using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.CreateMeasurementPointData;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;

internal static class MeasurementPointDataMappings
{
    public static readonly SortMappingDefinition<MeasurementPointDataListResponse, MeasurementPointData> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(MeasurementPointDataListResponse.Number), nameof(MeasurementPointData.Number)),
            new SortMapping(nameof(MeasurementPointDataListResponse.BeginTime), nameof(MeasurementPointData.BeginTime)),
            new SortMapping(nameof(MeasurementPointDataListResponse.EndTime), nameof(MeasurementPointData.EndTime)),
            new SortMapping(nameof(MeasurementPointDataListResponse.FlowDirection), nameof(MeasurementPointData.FlowDirectionType)),
            new SortMapping(nameof(MeasurementPointDataListResponse.GrossObservableVolume), nameof(MeasurementPointData.GrossObservableVolume)),
            new SortMapping(nameof(MeasurementPointDataListResponse.GrossStandardVolume), nameof(MeasurementPointData.GrossStandardVolume)),
            new SortMapping(nameof(MeasurementPointDataListResponse.Mass), nameof(MeasurementPointData.Mass)),
            new SortMapping(
                $"{nameof(MeasurementPointDataListResponse.MeasurementPoint)}.{nameof(MeasurementPointDataListResponse.MeasurementPoint.Id)}",
                $"{nameof(MeasurementPointData.MeasurementPoint)}.{nameof(MeasurementPointData.MeasurementPoint.Id)}"),
            new SortMapping(
                $"{nameof(MeasurementPointDataListResponse.MeasurementPoint)}.{nameof(MeasurementPointDataListResponse.MeasurementPoint.ControlPoint)}",
                $"{nameof(MeasurementPointData.MeasurementPoint)}.{nameof(MeasurementPointData.MeasurementPoint.ControlPoint)}"),
            new SortMapping(
                $"{nameof(MeasurementPointDataListResponse.Product)}.{nameof(MeasurementPointDataListResponse.Product.Code)}",
                $"{nameof(MeasurementPointData.Product)}.{nameof(MeasurementPointData.Product.Code)}")
        ]
    };

    public static MeasurementPointData ToEntity(this CreateMeasuringPointDataRequest request, string userName)
    => new()
    {
        Id = $"md_{Guid.CreateVersion7()}",
        MeasurementPointId = request.MeasurementPointId,
        Number = request.Number,
        BeginTime = request.BeginTime,
        EndTime = request.EndTime,
        FlowDirectionType = request.FlowDirectionType,
        ProductId = request.ProductId,
        TotalizerBeginGrossObservableVolume = request.TotalizerBeginGrossObservableVolume,
        TotalizerEndGrossObservableVolume = request.TotalizerEndGrossObservableVolume,
        TotalizerBeginGrossStandardVolume = request.TotalizerBeginGrossStandardVolume,
        TotalizerEndGrossStandardVolume = request.TotalizerEndGrossStandardVolume,
        TotalizerBeginMass = request.TotalizerBeginMass,
        TotalizerEndMass = request.TotalizerEndMass,
        GrossObservableVolume = request.GrossObservableVolume,
        GrossStandardVolume = request.GrossStandardVolume,
        Mass = request.Mass,
        AverageObservableDensity = request.AverageObservableDensity,
        AverageReferenceDensity = request.AverageReferenceDensity,
        AverageTemperature = request.AverageTemperature,
        AlcoholContent = request.AlcoholContent,
        BatchNumber = request.BatchNumber,
        ExternalId = request.ExternalId,
        CreatedBy = userName,
        CreatedAt = DateTime.UtcNow,
    };

    public static MeasurementPointDataResponse ToResponse(this MeasurementPointData mp)
     => new()
     {
         Id = mp.Id,
         MeasurementPoint = mp.MeasurementPoint is null ? null : new MeasurementPointDataMeasurementPointResponse
         {
             Id = mp.MeasurementPoint.Id,
             ControlPoint = mp.MeasurementPoint.ControlPoint,
             FullName = mp.MeasurementPoint.FullName,
             Ikunk = new MeasurementPointDataMeasurementPointIkunkResponse
             {
                 Id = mp.MeasurementPoint.Ikunk.Id,
                 FullName = mp.MeasurementPoint.Ikunk.FullName,
                 Warehouse = new MeasurementPointDataMeasurementPointIkunkWarehouseResponse
                 {
                     Id = mp.MeasurementPoint.Ikunk.Warehouse.Id,
                     FullName = mp.MeasurementPoint.Ikunk.Warehouse.FullName
                 }
             }
         },
         Number = mp.Number,
         BeginTime = mp.BeginTime,
         EndTime = mp.EndTime,
         FlowDirection = new EnumTypeResponse
         {
             Value = mp.FlowDirectionType,
             Description = mp.FlowDirectionType.GetDescription()
         },
         Product = mp.Product is null ? null : new MeasurementPointDataProducResponse
         {
             Id = mp.Product.Id,
             Code = mp.Product.Code,
             FullName = mp.Product.FullName
         },
         TotalizerBeginGrossObservableVolume = mp.TotalizerBeginGrossObservableVolume,
         TotalizerEndGrossObservableVolume = mp.TotalizerEndGrossObservableVolume,
         TotalizerBeginGrossStandardVolume = mp.TotalizerBeginGrossStandardVolume,
         TotalizerEndGrossStandardVolume = mp.TotalizerEndGrossStandardVolume,
         TotalizerBeginMass = mp.TotalizerBeginMass,
         TotalizerEndMass = mp.TotalizerEndMass,
         GrossObservableVolume = mp.GrossObservableVolume,
         GrossStandardVolume = mp.GrossStandardVolume,
         Mass = mp.Mass,
         AverageObservableDensity = mp.AverageObservableDensity,
         AverageReferenceDensity = mp.AverageReferenceDensity,
         AverageTemperature = mp.AverageTemperature,
         AlcoholContent = mp.AlcoholContent,
         BatchNumber = mp.BatchNumber,
         ExternalId = mp.ExternalId
     };
}
