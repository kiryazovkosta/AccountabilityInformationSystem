using System.ComponentModel.DataAnnotations;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.Encrypting;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Create;

public sealed class CreateMeasuringPointDataRequestHandler(
    ApplicationDbContext dbContext,
    EncryptionService encryptionService,
    UserContext userContext,
    MeasuringPointDataLinkService measuringPointDataLinkService)
{
    public async Task<Result<MeasurementPointDataResponse>> Handle(CreateMeasuringPointDataRequest request, CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Result<MeasurementPointDataResponse>.Failure(
                new Error("user", "Unauthorized"),
                ResultFailureType.Unauthorized);
        }

        if (!await dbContext.MeasurementPoints.AnyAsync(i => i.Id == request.MeasurementPointId, cancellationToken))
        {
            return Result<MeasurementPointDataResponse>.Failure(
                new Error("мeasurementPointId", "MeasurementPoint with specific id does not exists!"));
        }

        if (!await dbContext.Products.AnyAsync(i => i.Id == request.ProductId, cancellationToken))
        {
            return Result<MeasurementPointDataResponse>.Failure(
                new Error("productId", "Product with specific id does not exists!"));
        }

        if (await dbContext.MeasurementPointsData.AnyAsync(
            mpd => mpd.MeasurementPointId == request.MeasurementPointId &&
            mpd.Number == request.Number &&
            mpd.BeginTime == request.BeginTime &&
            mpd.EndTime == request.EndTime &&
            mpd.FlowDirectionType == request.FlowDirectionType, cancellationToken))
        {
            return Result<MeasurementPointDataResponse>.Failure(
                new Error("uniqueness", "Measurement point data with specific parameters already exists!"),
                ResultFailureType.Conflict);
        }

        MeasurementPointData measuringPointData = request.ToEntity(user.Email);
        measuringPointData.CreatedBy = encryptionService.Encrypt(user.Email);
        await dbContext.MeasurementPointsData.AddAsync(measuringPointData, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        MeasurementPointDataResponse measurementPointResponse = measuringPointData.ToResponse() with
        {
            Links = measuringPointDataLinkService.CreateLinksForMeasuringPointData(measuringPointData.Id)
        };

        return Result<MeasurementPointDataResponse>.Success(measurementPointResponse);
    }
}
