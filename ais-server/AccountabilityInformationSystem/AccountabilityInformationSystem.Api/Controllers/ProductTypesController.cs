using System.Dynamic;
using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Entities;
using AccountabilityInformationSystem.Api.Entities.Flow;
using AccountabilityInformationSystem.Api.Entities.Identity;
using AccountabilityInformationSystem.Api.Extensions;
using AccountabilityInformationSystem.Api.Models.Common;
using AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;
using AccountabilityInformationSystem.Api.Models.ProductTypes;
using AccountabilityInformationSystem.Api.Services.DataShaping;
using AccountabilityInformationSystem.Api.Services.Linking;
using AccountabilityInformationSystem.Api.Services.Sorting;
using AccountabilityInformationSystem.Api.Services.UserContexting;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Controllers;

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
