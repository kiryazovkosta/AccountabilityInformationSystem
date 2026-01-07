using System.Linq.Expressions;
using AccountabilityInformationSystem.Api.Entities.Flow;
using AccountabilityInformationSystem.Api.Extensions;
using AccountabilityInformationSystem.Api.Models.Flow.Ikunks;
using AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;
using AccountabilityInformationSystem.Api.Models.Products;

namespace AccountabilityInformationSystem.Api.Models.Flow.MeasurementPointsData;

public static class MeasurementPointDataQueries
{
    public static Expression<Func<MeasurementPointData, MeasurementPointDataListResponse>> ProjectToResponse()
    {
        return mp => new MeasurementPointDataListResponse
        {
            Id = mp.Id,
            MeasurementPoint = new()
            {
                Id = mp.MeasurementPoint.Id,
                ControlPoint = mp.MeasurementPoint.ControlPoint,
                FullName = mp.MeasurementPoint.FullName,
                Ikunk = new()
                {
                    Id = mp.MeasurementPoint.Ikunk.Id,
                    FullName = mp.MeasurementPoint.Ikunk.FullName,
                    Warehouse = new()
                    {
                        Id = mp.MeasurementPoint.Ikunk.Warehouse.Id,
                        FullName = mp.MeasurementPoint.Ikunk.Warehouse.FullName
                    }
                }
            },
            Number = mp.Number,
            BeginTime = mp.BeginTime,
            EndTime = mp.EndTime,
            FlowDirection = new EnumTypeResponse()
            {
                Value = mp.FlowDirectionType,
                Description = mp.FlowDirectionType.GetDescription()
            },
            Product = new()
            {
                Id = mp.Product.Id,
                Code = mp.Product.Code,
                FullName = mp.Product.FullName
            },
            GrossObservableVolume = mp.GrossObservableVolume,
            GrossStandardVolume = mp.GrossStandardVolume,
            Mass = mp.Mass
        };
    }

    public static Expression<Func<MeasurementPointData, MeasurementPointDataResponse>> ProjectToDatailsResponse()
    {
        return mp => new MeasurementPointDataResponse
        {
            Id = mp.Id,
            MeasurementPoint = new()
            {
                Id = mp.MeasurementPoint.Id,
                ControlPoint = mp.MeasurementPoint.ControlPoint,
                FullName = mp.MeasurementPoint.FullName,
                Ikunk = new()
                {
                    Id = mp.MeasurementPoint.Ikunk.Id,
                    FullName = mp.MeasurementPoint.Ikunk.FullName,
                    Warehouse = new()
                    {
                        Id = mp.MeasurementPoint.Ikunk.Warehouse.Id,
                        FullName = mp.MeasurementPoint.Ikunk.Warehouse.FullName
                    }
                }
            },
            Number = mp.Number,
            BeginTime = mp.BeginTime,
            EndTime = mp.EndTime,
            FlowDirection = new EnumTypeResponse()
            {
                Value = mp.FlowDirectionType,
                Description = mp.FlowDirectionType.GetDescription()
            },
            Product = new()
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
}
