using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.Linking;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetAll;

internal static class GetMeasuringPointsLinks
{
    private const string GetMeasuringPoints = nameof(MeasuringPointsController.GetMeasuringPoints);
    private const string CreateMeasuringPoint = nameof(MeasuringPointsController.CreateMeasuringPoint);

    public static List<LinkResponse> CreateLinksForMeasuringPoints(
        this LinkService linkService,
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
}
