using System.Dynamic;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Domain.Entities;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Features.ProductTypes.Shared;
using AccountabilityInformationSystem.Api.Features.ProductTypes.CreateProductType;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.ProductTypes;

[ApiController]
[Route("api/product-types")]
[Authorize]
public sealed class ProductTypesController(
    ApplicationDbContext dbContext,
    UserContext userContext) : ControllerBase
{
    [HttpGet]
    [MapToApiVersion(1.0)]
    [Produces(typeof(PaginationResponse<ProductTypeResponse>))]
    public async Task<IActionResult> GetProductTypes(
        [FromQuery] QueryParameters query,
        SortMappingProvider sortMappingProvider,
        DataShapingService dataShapingService,
        CancellationToken cancellationToken)
    {
        if (!sortMappingProvider.ValidateMappings<ProductTypeResponse, ProductType>(query.Sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid sort parameter. {query.Sort}");
        }

        if (!dataShapingService.Validate<ProductTypeResponse>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid fields parameter. {query.Fields}");
        }

        query.Search = query.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<ProductTypeResponse, ProductType>();

        IQueryable<ProductTypeResponse> productTypeQuery = dbContext
            .ProductTypes
            .Where(pt =>
                query.Search == null ||
                EF.Functions.Like(pt.Name, $"%{query.Search}%") ||
                EF.Functions.Like(pt.FullName, $"%{query.Search}%")
            )
            .ApplySort(query.Sort, sortMappings)
            .AsNoTracking()
            .Select(ProductTypeQueries.ProjectToResponse());

        PaginationResponse<ExpandoObject> response = new()
        {
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = await productTypeQuery.CountAsync(cancellationToken),
            Items = dataShapingService.ShapeCollectionData(
                await productTypeQuery
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync(cancellationToken),
                query.Fields)
        };

        return Ok(response);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductType(
    string id,
    [FromQuery] FieldsOnlyQueryParameters query,
    DataShapingService dataShapingService,
    CancellationToken cancellationToken)
    {
        if (!dataShapingService.Validate<ProductTypeResponse>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid fields parameter. {query.Fields}");
        }

        ProductTypeResponse? productTypeResponse = await dbContext
            .ProductTypes
            .AsNoTracking()
            .Select(ProductTypeQueries.ProjectToResponse())
            .FirstOrDefaultAsync(mp => mp.Id == id, cancellationToken);
        if (productTypeResponse is null)
        {
            return NotFound();
        }

        ExpandoObject shapedResponse = dataShapingService.ShapeData(productTypeResponse, query.Fields);
        return Ok(shapedResponse);
    }

    [HttpPost]
    public async Task<ActionResult<ProductTypeResponse>> CreateProductType(
        [FromBody] CreateProductTypeRequest request,
        IValidator<CreateProductTypeRequest> validator,
        CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Unauthorized");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        if (await dbContext.ProductTypes.AnyAsync(mp => mp.Name == request.Name, cancellationToken))
        {
            return Problem(
                detail: "Product type with specific name already exists!",
                statusCode: StatusCodes.Status409Conflict);
        }

        ProductType productType = request.ToEntity(user.Email);
        await dbContext.ProductTypes.AddAsync(productType, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        ProductTypeResponse productTypeResponse = productType.ToResponse();

        return CreatedAtAction(
            actionName: nameof(GetProductType),
            routeValues: new { id = productType.Id },
            value: productTypeResponse);
    }
}
