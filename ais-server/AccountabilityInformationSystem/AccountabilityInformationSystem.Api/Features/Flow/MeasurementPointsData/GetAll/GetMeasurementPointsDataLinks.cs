using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.Linking;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.GetAll;

internal static class GetMeasurementPointsDataLinks
{
    private const string GetMeasuringPointsData = nameof(MeasurementPointsDataController.GetMeasuringPointsData);
    private const string CreateMeasuringPointData = nameof(MeasurementPointsDataController.CreateMeasuringPointData);

    public static List<LinkResponse> CreateLinksForMeasuringPointsData(
        this LinkService linkService,
        GetMeasurementPointsDataRequest query,
        bool hasNextPage,
        bool hasPreviousPage)
    {
        List<LinkResponse> links =
        [
            linkService.Create(GetMeasuringPointsData, "self", HttpMethods.Get, new
            {
                page = query.Page,
                pageSize = query.PageSize,
                fields = query.Fields,
                q = query.Search,
                sort = query.Sort,
                warehouses = query.Warehouses,
                ikunks = query.Ikunks,
                measurementpoints = query.MeasurementPoints,
                begin = query.Begin,
                end = query.End,
                flowDirection = query.FlowDirection,
                products = query.Products
            }),
            linkService.Create(CreateMeasuringPointData, "create", HttpMethods.Post),
        ];

        if (hasNextPage)
        {
            links.Add(linkService.Create(GetMeasuringPointsData, "next-page", HttpMethods.Get, new
            {
                page = query.Page + 1,
                pageSize = query.PageSize,
                fields = query.Fields,
                q = query.Search,
                sort = query.Sort,
                warehouses = query.Warehouses,
                ikunks = query.Ikunks,
                measurementpoints = query.MeasurementPoints,
                begin = query.Begin,
                end = query.End,
                flowDirection = query.FlowDirection,
                products = query.Products
            }));
        }

        if (hasPreviousPage)
        {
            links.Add(linkService.Create(GetMeasuringPointsData, "previous-page", HttpMethods.Get, new
            {
                page = query.Page - 1,
                pageSize = query.PageSize,
                fields = query.Fields,
                q = query.Search,
                sort = query.Sort,
                warehouses = query.Warehouses,
                ikunks = query.Ikunks,
                measurementpoints = query.MeasurementPoints,
                begin = query.Begin,
                end = query.End,
                flowDirection = query.FlowDirection,
                products = query.Products
            }));
        }

        return links;
    }
}
