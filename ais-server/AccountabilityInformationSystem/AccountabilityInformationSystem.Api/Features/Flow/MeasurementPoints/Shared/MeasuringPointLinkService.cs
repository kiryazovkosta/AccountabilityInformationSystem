using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.Linking;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;

public sealed class MeasuringPointLinkService(LinkService linkService)
{
    private const string GetMeasuringPoint = nameof(MeasuringPointsController.GetMeasuringPoint);
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
}
