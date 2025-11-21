using System.Dynamic;
using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Extensions;
using AccountabilityInformationSystem.Api.Models.Common;
using AccountabilityInformationSystem.Api.Models.ExciseNomenclatures;
using AccountabilityInformationSystem.Api.Models.ExciseNomenclatures.ApCodes;
using AccountabilityInformationSystem.Api.Services.DataShaping;
using AccountabilityInformationSystem.Api.Services.Sorting;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Controllers.ExciseNoms;

[Authorize]
[ApiController]
public abstract class ExciseNomenclatureController<TEntity>(
    ApplicationDbContext dbContext) : ControllerBase
    where TEntity : class, IEntity, IExciseEntity
{
    [HttpGet]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> GetCodes(
        [FromQuery] ExciseNomenclatureQueryParameters query,
        SortMappingProvider sortMappingProvider,
        DataShapingService dataShapingService,
        CancellationToken cancellationToken)
    {
        if (!sortMappingProvider.ValidateMappings<ExciseNomenclatureResponse, TEntity>(query.Sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid sort parameter. {query.Sort}");
        }

        if (!dataShapingService.Validate<ApCodeResponse>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid fields parameter. {query.Fields}");
        }

        query.Search = query.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<ExciseNomenclatureResponse, TEntity>();

        IQueryable<ExciseNomenclatureResponse> queryable = dbContext
            .Set<TEntity>()
            .Where(pt =>
                query.Search == null ||
                EF.Functions.Like(pt.BgDescription, $"%{query.Search}%") ||
                EF.Functions.Like(pt.DescriptionEn, $"%{query.Search}%")
            )
            .Where(en => query.IsUsed == null || en.IsUsed == query.IsUsed)
            .ApplySort(query.Sort, sortMappings)
            .AsNoTracking()
            .Select(ExciseNomenclatureQueries.ProjectToResponse<TEntity>());

        PaginationResponse<ExpandoObject> response = new()
        {
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = await queryable.CountAsync(cancellationToken),
            Items = dataShapingService.ShapeCollectionData(
                await queryable
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync(cancellationToken),
                query.Fields)
        };

        return Ok(response);
    }


    //[HttpGet("{id}")]
    //public async Task<IActionResult> GetCode(
    //    string id,
    //    [FromQuery] FieldsOnlyQueryParameters query,
    //    DataShapingService dataShapingService,
    //    CancellationToken cancellationToken)
    //{
    //    if (!dataShapingService.Validate<ProductTypeResponse>(query.Fields))
    //    {
    //        return Problem(
    //            statusCode: StatusCodes.Status400BadRequest,
    //            detail: $"Invalid fields parameter. {query.Fields}");
    //    }

    //    ProductTypeResponse? productTypeResponse = await dbContext
    //        .ProductTypes
    //        .AsNoTracking()
    //        .Select(ProductTypeQueries.ProjectToResponse())
    //        .FirstOrDefaultAsync(mp => mp.Id == id, cancellationToken);
    //    if (productTypeResponse is null)
    //    {
    //        return NotFound();
    //    }

    //    ExpandoObject shapedResponse = dataShapingService.ShapeData(productTypeResponse, query.Fields);
    //    return Ok(shapedResponse);
    //}

    //[HttpPost]
    //public async Task<ActionResult<ProductTypeResponse>> Create(
    //    [FromBody] CreateProductTypeRequest request,
    //    IValidator<CreateProductTypeRequest> validator,
    //    CancellationToken cancellationToken)
    //{
    //    User? user = await userContext.GetUserAsync(cancellationToken);
    //    if (user is null)
    //    {
    //        return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Unauthorized");
    //    }

    //    await validator.ValidateAndThrowAsync(request, cancellationToken);

    //    if (await dbContext.ProductTypes.AnyAsync(mp => mp.Name == request.Name, cancellationToken))
    //    {
    //        return Problem(
    //            detail: "Product type with specific name already exists!",
    //            statusCode: StatusCodes.Status409Conflict);
    //    }

    //    ProductType productType = request.ToEntity(user.Email);
    //    await dbContext.ProductTypes.AddAsync(productType, cancellationToken);
    //    await dbContext.SaveChangesAsync(cancellationToken);
    //    ProductTypeResponse productTypeResponse = productType.ToResponse();

    //    return CreatedAtAction(
    //        actionName: nameof(GetProductType),
    //        routeValues: new { id = productType.Id },
    //        value: productTypeResponse);
    //}
}
