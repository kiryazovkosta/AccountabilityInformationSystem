using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.ProductTypes.Create;
using AccountabilityInformationSystem.Api.Features.ProductTypes.GetAll;
using AccountabilityInformationSystem.Api.Features.ProductTypes.GetById;
using AccountabilityInformationSystem.Api.Features.ProductTypes.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace AccountabilityInformationSystem.Api.Features.ProductTypes;

[ApiController]
[Route("api/product-types")]
[Authorize]
public sealed class ProductTypesController(
    IMessageBus bus) : ControllerBase
{
    [HttpGet]
    [MapToApiVersion(1.0)]
    [Produces(typeof(PaginationResponse<ProductTypeResponse>))]
    public async Task<IActionResult> GetProductTypes(
        [FromQuery] GetProductTypesRequest request,
        CancellationToken cancellationToken)
    {
        Result<PaginationResponse<ExpandoObject>> response =
            await bus.InvokeAsync<Result<PaginationResponse<ExpandoObject>>>(request, cancellationToken);
        return response.ToActionResult();
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductType(
    string id,
    [FromQuery] FieldsOnlyQueryParameters query,
    CancellationToken cancellationToken)
    {
        GetProductTypeByIdRequest request = new(id, query.Fields);
        Result<ExpandoObject> result = await bus.InvokeAsync<Result<ExpandoObject>>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductType(
        [FromBody] CreateProductTypeRequest request,
        IValidator<CreateProductTypeRequest> validator,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        Result<ProductTypeResponse> result = await bus.InvokeAsync<Result<ProductTypeResponse>>(request, cancellationToken);
        if (result.IsFailure)
        {
            return result.ToActionResult();
        }

        return CreatedAtAction(nameof(GetProductType), new { id = result.Value }, result.Value);
    }
}
