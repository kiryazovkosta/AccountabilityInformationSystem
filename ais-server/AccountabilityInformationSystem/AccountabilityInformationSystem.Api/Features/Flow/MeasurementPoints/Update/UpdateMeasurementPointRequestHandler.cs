using System.ComponentModel.DataAnnotations;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Update;

public sealed class UpdateMeasurementPointRequestHandler(
    ApplicationDbContext dbContext,
    UserContext userContext)
{
    public async Task<Result> Handle(UpdateMeasurementPointRequest request, CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Result.Failure(new Error("auth", "Unauthorized"), ResultFailureType.Unauthorized);
        }

        if (request.IkunkId is not null 
            && !await dbContext.Ikunks.AnyAsync(i => i.Id == request.IkunkId, cancellationToken))
        {
            return Result.Failure(new Error("auth", "Ikunk with specific id does not exists!"));
        }

        MeasurementPoint? measuringPoint = 
            await dbContext.MeasurementPoints.FirstOrDefaultAsync(mp => mp.Id == request.Id, cancellationToken);
        if (measuringPoint is null)
        {
            return Result.Failure(
                new Error("id", "Measurement point with specific id does not exists!"), 
                ResultFailureType.NotFound);
        }

        if (await dbContext.MeasurementPoints.AnyAsync(
            mp => (mp.Name == request.Name || mp.ControlPoint == request.ControlPoint) 
                && mp.Id != request.Id, 
            cancellationToken))
        {
            return Result.Failure(
                new Error("unique", "Measurement point with specific name or control point already exists!"), 
                ResultFailureType.Conflict);
        }

        measuringPoint.UpdateFromRequest(request, user.Email);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ResultSuccessType.NoContent);
    }
}
