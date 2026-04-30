using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetById;

public sealed class GetGetMeasuringPointRequestHandler(
    ApplicationDbContext dbContext,
    DataShapingService dataShapingService,
    MeasuringPointLinkService measuringPointDataLinkService)
{
    public async Task<Result<ExpandoObject>> Handle(GetGetMeasuringPointRequest request, CancellationToken cancellationToken)
    {
        if (!dataShapingService.Validate<MeasurementPointResponse>(request.Fields))
        {
            return Result<ExpandoObject>.Failure(
                new Error("fields", $"Invalid fields parameter. {request.Fields}"));
        }

        MeasurementPointResponse? measuringPointResponse = await dbContext
            .MeasurementPoints
            .AsNoTracking()
            .Select(MeasurementPointQueries.ProjectToResponse())
            .FirstOrDefaultAsync(mp => mp.Id == request.Id, cancellationToken);
        if (measuringPointResponse is null)
        {
            return Result<ExpandoObject>.Failure(
                new Error("id", "No matching records were found for your search criteria."),
                ResultFailureType.NotFound);
        }

        ExpandoObject shapedResponse = dataShapingService.ShapeData(measuringPointResponse, request.Fields);
        if (request.IncludeLinks)
        {
            shapedResponse.TryAdd("links", measuringPointDataLinkService.CreateLinksForMeasuringPoint(request.Id, request.Fields));
        }

        return Result<ExpandoObject>.Success(shapedResponse);
    } 
}
