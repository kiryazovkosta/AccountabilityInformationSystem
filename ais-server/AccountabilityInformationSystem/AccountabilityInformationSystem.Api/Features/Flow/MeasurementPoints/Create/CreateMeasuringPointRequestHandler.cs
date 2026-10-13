using System.ComponentModel.DataAnnotations;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Create;

public sealed class CreateMeasuringPointRequestHandler(
    ApplicationDbContext dbContext,
    MeasuringPointLinkService measuringPointLinkService)
{
    public async Task<Result<MeasurementPointResponse>> Handle(CreateMeasuringPointRequest request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Ikunks.AnyAsync(i => i.Id == request.IkunkId, cancellationToken))
        {
            return Result<MeasurementPointResponse>.Failure(
                new Error("ikunkId", "Ikunk with specific id does not exists!"));
        }

        if (await dbContext.MeasurementPoints.AnyAsync(mp => mp.Name == request.Name || mp.ControlPoint == request.ControlPoint, cancellationToken))
        {
            return Result<MeasurementPointResponse>.Failure(
                new Error("controlPoint", "Measurement point with specific name or control point already exists!"),
                ResultFailureType.Conflict);
        }

        MeasurementPoint measuringPoint = request.Adapt<MeasurementPoint>();
        await dbContext.MeasurementPoints.AddAsync(measuringPoint, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        MeasurementPointResponse measurementPointResponse = measuringPoint.Adapt<MeasurementPointResponse>() with
        {
            Links = measuringPointLinkService.CreateLinksForMeasuringPoint(measuringPoint.Id)
        };

        return Result<MeasurementPointResponse>.Success(measurementPointResponse);
    }
}
