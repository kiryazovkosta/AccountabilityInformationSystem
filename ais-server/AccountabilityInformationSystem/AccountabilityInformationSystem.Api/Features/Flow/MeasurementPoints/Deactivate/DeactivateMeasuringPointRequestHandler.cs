using System.Diagnostics;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Deactivate;

public sealed class DeactivateMeasuringPointRequestHandler(
    ApplicationDbContext dbContext)
{
    public async Task<Result> Handle(
        DeactivateMeasuringPointRequest request, 
        CancellationToken cancellationToken)
    {
        MeasurementPoint? measuringPoint = await dbContext.MeasurementPoints
            .FirstOrDefaultAsync(mp => mp.Id == request.Id, cancellationToken);
        if (measuringPoint is null)
        {
            return Result.Failure(
                new Error("Id", "Measurement point with specific id does not exists!"),
                ResultFailureType.NotFound);
        }

        measuringPoint.ActiveTo = request.ActiveTo;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ResultSuccessType.NoContent);
    }
}
