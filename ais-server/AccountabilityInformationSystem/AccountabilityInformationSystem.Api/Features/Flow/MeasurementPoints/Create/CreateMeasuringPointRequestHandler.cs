using System.ComponentModel.DataAnnotations;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Create;

public sealed class CreateMeasuringPointRequestHandler(
    ApplicationDbContext dbContext,
    UserContext userContext,
    MeasuringPointLinkService measuringPointLinkService)
{
    public async Task<Result<MeasurementPointResponse>> Handle(CreateMeasuringPointRequest request, CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Result<MeasurementPointResponse>.Failure(
                new Error("user", "Unauthorized"),
                ResultFailureType.Unauthorized);
        }

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

        MeasurementPoint measuringPoint = request.ToEntity(user.Email);
        await dbContext.MeasurementPoints.AddAsync(measuringPoint, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        MeasurementPointResponse measurementPointResponse = measuringPoint.ToResponse() with
        {
            Links = measuringPointLinkService.CreateLinksForMeasuringPoint(measuringPoint.Id)
        };

        return Result<MeasurementPointResponse>.Success(measurementPointResponse);
    }
}
