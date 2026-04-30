using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;

public static class QueryableMeasurementPointDataExtensions
{
    public static IQueryable<MeasurementPointData> SetMeasuringPoints(this IQueryable<MeasurementPointData> query,
        List<string>? warehouses,
        List<string>? ikunks,
        List<string>? measurementPoints)
    {
        if (measurementPoints is not null && measurementPoints.Any())
        {
            query = query.Where(mpd => measurementPoints.Contains(mpd.MeasurementPointId));
        }
        else if (ikunks is not null && ikunks.Any())
        {
            query = query.Where(mpd => ikunks.Contains(mpd.MeasurementPoint.IkunkId));
        }
        else if (warehouses is not null && warehouses.Any())
        {
            query = query.Where(mpd => warehouses.Contains(mpd.MeasurementPoint.Ikunk.WarehouseId));
        }

        return query;
    }

    public static IQueryable<MeasurementPointData> SetTimePeriod(this IQueryable<MeasurementPointData> query,
        DateTime? beginTime,
        DateTime? endTime)
    {
        if (beginTime.HasValue && endTime.HasValue)
        {
            query = query.Where(mpd => mpd.EndTime >= beginTime && mpd.EndTime <= endTime);
        }

        return query;
    }
}
