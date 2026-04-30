using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetAll;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetAllV2;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.Linking;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;

public sealed class MeasuringPointLinkService(LinkService linkService)
{
    private const string GetMeasuringPoint = nameof(MeasuringPointsController.GetMeasuringPoint);
    private const string GetMeasuringPoints = nameof(MeasuringPointsController.GetMeasuringPoints);
    private const string GetMeasuringPointsV2 = nameof(MeasuringPointsController.GetMeasuringPointsV2);
    private const string CreateMeasuringPoint = nameof(MeasuringPointsController.CreateMeasuringPoint);
    private const string UpdateMeasurementPoint = nameof(MeasuringPointsController.UpdateMeasurementPoint);
    private const string DeactivateMeasurementPoint = nameof(MeasuringPointsController.DeactivateMeasurementPoint);

    public List<LinkResponse> CreateLinksForMeasuringPoint(string id, string? fields = null)
    {
        return
        [
            linkService.Create(GetMeasuringPoint, "self", HttpMethods.Get, new { id, fields }),
            linkService.Create(UpdateMeasurementPoint, "update", HttpMethods.Put, new { id }),
            linkService.Create(DeactivateMeasurementPoint, "deactivate", HttpMethods.Put, new { id })
        ];
    }

    public List<LinkResponse> CreateLinksForMeasuringPoints(
        GetMeasuringPointsRequest parameters,
        bool hasNextPage,
        bool hasPreviousPage)
    {
        List<LinkResponse> links =
        [
            linkService.Create(GetMeasuringPoints, "self", HttpMethods.Get, new
            {
                page = parameters.Page,
                pageSize = parameters.PageSize,
                fields = parameters.Fields,
                q = parameters.Search,
                sort = parameters.Sort,
                ikunkid = parameters.IkunkId,
                direction = parameters.FlowDirection,
                transport = parameters.Transport
            }),
            linkService.Create(CreateMeasuringPoint, "create", HttpMethods.Post),
        ];

        if (hasNextPage)
        {
            links.Add(linkService.Create(GetMeasuringPoints, "next-page", HttpMethods.Get, new
            {
                page = parameters.Page + 1,
                pageSize = parameters.PageSize,
                fields = parameters.Fields,
                q = parameters.Search,
                sort = parameters.Sort,
                ikunkid = parameters.IkunkId,
                direction = parameters.FlowDirection,
                transport = parameters.Transport
            }));
        }

        if (hasPreviousPage)
        {
            links.Add(linkService.Create(GetMeasuringPoints, "previous-page", HttpMethods.Get, new
            {
                page = parameters.Page - 1,
                pageSize = parameters.PageSize,
                fields = parameters.Fields,
                q = parameters.Search,
                sort = parameters.Sort,
                ikunkid = parameters.IkunkId,
                direction = parameters.FlowDirection,
                transport = parameters.Transport
            }));
        }

        return links;
    }

    public List<LinkResponse> CreateLinksForMeasuringPointsV2(
        GetMeasuringPointsV2Request parameters,
        bool hasNextPage,
        bool hasPreviousPage)
    {
        List<LinkResponse> links =
        [
            linkService.Create(GetMeasuringPointsV2, "self", HttpMethods.Get, new
            {
                page = parameters.Page,
                pageSize = parameters.PageSize,
                fields = parameters.Fields,
                q = parameters.Search,
                sort = parameters.Sort,
                ikunkid = parameters.IkunkId,
                direction = parameters.FlowDirection,
                transport = parameters.Transport
            }),
            linkService.Create(CreateMeasuringPoint, "create", HttpMethods.Post),
        ];

        if (hasNextPage)
        {
            links.Add(linkService.Create(GetMeasuringPointsV2, "next-page", HttpMethods.Get, new
            {
                page = parameters.Page + 1,
                pageSize = parameters.PageSize,
                fields = parameters.Fields,
                q = parameters.Search,
                sort = parameters.Sort,
                ikunkid = parameters.IkunkId,
                direction = parameters.FlowDirection,
                transport = parameters.Transport
            }));
        }

        if (hasPreviousPage)
        {
            links.Add(linkService.Create(GetMeasuringPointsV2, "previous-page", HttpMethods.Get, new
            {
                page = parameters.Page - 1,
                pageSize = parameters.PageSize,
                fields = parameters.Fields,
                q = parameters.Search,
                sort = parameters.Sort,
                ikunkid = parameters.IkunkId,
                direction = parameters.FlowDirection,
                transport = parameters.Transport
            }));
        }

        return links;
    }
}
