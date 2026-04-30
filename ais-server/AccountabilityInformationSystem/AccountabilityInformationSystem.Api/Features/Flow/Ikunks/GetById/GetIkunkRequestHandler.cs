using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Flow.Ikunks.GetById;

public sealed class GetIkunkRequestHandler(
    ApplicationDbContext dbContext,
    DataShapingService dataShapingService)
{
    public async Task<Result<ExpandoObject>> Handle(
        GetIkunkRequest request,
        CancellationToken cancellationToken)
    {
        if (!dataShapingService.Validate<IkunkResponse>(request.Fields))
        {
            return Result<ExpandoObject>.Failure(
                new Error("fields", $"Invalid fields parameter. {request.Fields}"));
        }

        IkunkResponse? ikunkResponse = await dbContext.Ikunks
            .AsNoTracking()
            .ProjectToType<IkunkResponse>()
            .FirstOrDefaultAsync(ik => ik.Id == request.Id, cancellationToken);

        if (ikunkResponse is null)
        {
            return Result<ExpandoObject>.Failure(
                new Error("id", "No matching records were found for your search criteria."),
                ResultFailureType.NotFound);
        }

        return Result<ExpandoObject>.Success(dataShapingService.ShapeData(ikunkResponse, request.Fields));
    }
}
