using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.GetById;

public sealed class GetMeasuringPointDataRequestHandler(
    ApplicationDbContext dbContext,
    DataShapingService dataShapingService,
    MeasuringPointDataLinkService measuringPointDataLinkService)
{
    public async Task<Result<ExpandoObject>> Handle(GetMeasuringPointDataRequest request, CancellationToken cancellationToken)
    {
        if (!dataShapingService.Validate<MeasurementPointDataResponse>(request.Fields))
        {
            return Result<ExpandoObject>.Failure(
                new Error("fields", $"Invalid fields parameter. {request.Fields}"));
        }

        MeasurementPointDataResponse? measuringPointDataResponse = await dbContext
            .MeasurementPointsData
            .AsNoTracking()
            .Select(MeasurementPointDataQueries.ProjectToDatailsResponse())
            .FirstOrDefaultAsync(mp => mp.Id == request.Id, cancellationToken);
        if (measuringPointDataResponse is null)
        {
            return Result<ExpandoObject>.Failure(
                new Error("id", "No matching records were found for your search criteria."),
                ResultFailureType.NotFound);
        }

        ExpandoObject shapedResponse = dataShapingService.ShapeData(measuringPointDataResponse, request.Fields);
        if (request.IncludeLinks)
        {
            shapedResponse.TryAdd("links", measuringPointDataLinkService.CreateLinksForMeasuringPointData(request.Id, request.Fields));
        }

        return Result<ExpandoObject>.Success(shapedResponse);
    }
}
