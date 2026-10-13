using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.Linking;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;

public sealed class MeasuringPointDataLinkService(LinkService linkService)
{
    private const string GetMeasuringPointData = nameof(MeasurementPointsDataController.GetMeasuringPointData);

    public List<LinkResponse> CreateLinksForMeasuringPointData(string id, string? fields = null)
    {
        List<LinkResponse> links =
        [
            linkService.Create(GetMeasuringPointData, "self", HttpMethods.Get, new { id, fields })
        ];

        return links;
    }
}
