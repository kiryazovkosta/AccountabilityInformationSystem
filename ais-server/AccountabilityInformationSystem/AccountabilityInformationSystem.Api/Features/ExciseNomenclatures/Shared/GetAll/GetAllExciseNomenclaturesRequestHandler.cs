using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.GetAll;

public class GetAllExciseNomenclaturesRequestHandler<TEntity>(
    SortMappingProvider sortMappingProvider,
    DataShapingService dataShapingService,
    ApplicationDbContext dbContext)
        where TEntity : AuditableEntity, IEntity, IExciseEntity, new()
{ 
    public async Task<Result<PaginationResponse<ExpandoObject>>> Handle(
        GetAllExciseNomenclaturesRequest<TEntity> request, 
        CancellationToken cancellationToken)
    {
        if (!sortMappingProvider.ValidateMappings<ExciseNomenclatureResponse, TEntity>(request.Query.Sort))
        {
            return Result<PaginationResponse<ExpandoObject>>.Failure(
                new Error("", $"Invalid sort parameter. {request.Query.Sort}"));
        }

        if (!dataShapingService.Validate<ExciseNomenclatureResponse>(request.Query.Fields))
        {
            return Result<PaginationResponse<ExpandoObject>>.Failure(
                new Error("", $"Invalid fields parameter. {request.Query.Fields}"));
        }

        request.Query.Search = request.Query.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<ExciseNomenclatureResponse, TEntity>();

        IQueryable<ExciseNomenclatureResponse> queryable = dbContext
            .Set<TEntity>()
            .Where(pt =>
                request.Query.Search == null ||
                EF.Functions.Like(pt.BgDescription, $"%{request.Query.Search}%") ||
                EF.Functions.Like(pt.DescriptionEn, $"%{request.Query.Search}%")
            )
            .Where(en => request.Query.IsUsed == null || en.IsUsed == request.Query.IsUsed)
            .ApplySort(request.Query.Sort, sortMappings)
            .AsNoTracking()
            .Select(ExciseNomenclatureQueries.ProjectToResponse<TEntity>());

        PaginationResponse<ExpandoObject> response = new()
        {
            Page = request.Query.Page,
            PageSize = request.Query.PageSize,
            TotalCount = await queryable.CountAsync(cancellationToken),
            Items = dataShapingService.ShapeCollectionData(
                await queryable
                    .Skip((request.Query.Page - 1) * request.Query.PageSize)
                    .Take(request.Query.PageSize)
                    .ToListAsync(cancellationToken),
                request.Query.Fields)
        };

        return Result<PaginationResponse<ExpandoObject>>.Success(response);
    }
}
