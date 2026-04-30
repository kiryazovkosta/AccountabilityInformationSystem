using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.GetById;

public class GetExciseNomenclatureByIdRequestHandler<TEntity>(
    DataShapingService dataShapingService,
    ApplicationDbContext dbContext)
        where TEntity : AuditableEntity, IEntity, IExciseEntity, new()
{
    public async Task<Result<ExpandoObject>> Handle(
        GetExciseNomenclatureByIdRequest<TEntity> request,
        CancellationToken cancellationToken)
    {
        if (!dataShapingService.Validate<ExciseNomenclatureResponse>(request.Query.Fields))
        {
            return Result<ExpandoObject>.Failure(
                new Error("", $"Invalid fields parameter. {request.Query.Fields}"));
        }

        ExciseNomenclatureResponse? exciseNomenclatureResponse = await dbContext
            .Set<TEntity>()
            .AsNoTracking()
            .Select(ExciseNomenclatureQueries.ProjectToResponse<TEntity>())
            .FirstOrDefaultAsync(mp => mp.Id == request.Id, cancellationToken);
        if (exciseNomenclatureResponse is null)
        {
            return Result<ExpandoObject>.Failure(
                new Error("Id", $"{typeof(TEntity).Name} with specific id does not exist!"),
                ResultFailureType.NotFound);
        }

        ExpandoObject shapedResponse = dataShapingService.ShapeData(exciseNomenclatureResponse, request.Query.Fields);
        return Result<ExpandoObject>.Success(shapedResponse);
    }
}
