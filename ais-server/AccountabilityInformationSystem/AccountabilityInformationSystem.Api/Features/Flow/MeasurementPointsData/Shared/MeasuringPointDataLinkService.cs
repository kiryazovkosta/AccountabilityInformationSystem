using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.GetAll;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.Linking;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;

public sealed class MeasuringPointDataLinkService(LinkService linkService)
{
    private const string GetMeasuringPointData = nameof(MeasurementPointsDataController.GetMeasuringPointData);
    private const string GetMeasuringPointsData = nameof(MeasurementPointsDataController.GetMeasuringPointsData);
    private const string CreateMeasuringPointData = nameof(MeasurementPointsDataController.CreateMeasuringPointData);

    public List<LinkResponse> CreateLinksForMeasuringPointData(string id, string? fields = null)
    {
        List<LinkResponse> links =
        [
            linkService.Create(nameof(GetMeasuringPointData), "self", HttpMethods.Get, new { id, fields })
        ];

        return links;
    }

    public List<LinkResponse> CreateLinksForMeasuringPointsData(GetMeasurementPointsDataRequest query, bool hasNextPage, bool hasPreviousPage)
    {
        List<LinkResponse> links =
       [
           linkService.Create(nameof(GetMeasuringPointsData), "self", HttpMethods.Get, new
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
            linkService.Create(nameof(CreateMeasuringPointData), "create", HttpMethods.Post),
        ];

        if (hasNextPage)
        {
            links.Add(linkService.Create(nameof(GetMeasuringPointsData), "next-page", HttpMethods.Get, new
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
            links.Add(
                linkService.Create(nameof(GetMeasuringPointsData), "previous-page", HttpMethods.Get, new
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
                })
            );
        }

        return links;
    }
}
