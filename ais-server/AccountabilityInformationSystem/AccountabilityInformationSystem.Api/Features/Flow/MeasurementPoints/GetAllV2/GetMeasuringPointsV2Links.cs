using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.Linking;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetAllV2;

internal static class GetMeasuringPointsV2Links
{
    private const string GetMeasuringPointsV2 = nameof(MeasuringPointsController.GetMeasuringPointsV2);
    private const string CreateMeasuringPoint = nameof(MeasuringPointsController.CreateMeasuringPoint);

    public static List<LinkResponse> CreateLinksForMeasuringPointsV2(
        this LinkService linkService,
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
