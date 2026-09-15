using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.Create;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.Delete;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.GetAll;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.GetById;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.Shared;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands;

[ApiController]
[Route("api/family/warranty-brands")]
[ApiVersion("1.0")]
[Authorize]
public sealed class WarrantyBrandsController(IMessageBus bus) : ApiController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetWarrantyBrandById(
    string id,
    string? fields,
    CancellationToken cancellationToken)
    {
        GetWarrantyBrandByIdRequest request = new(id, fields);
        Result<ExpandoObject> result = await bus.InvokeAsync<Result<ExpandoObject>>(request, cancellationToken);
        return result.ToActionResult();
    }


    [HttpGet]
    [Produces(typeof(WarrantyBrandsCollectionResponse))]
    public async Task<IActionResult> GetWarrantyBrands(CancellationToken cancellationToken)
    {
        Result<WarrantyBrandsCollectionResponse> result =
            await bus.InvokeAsync<Result<WarrantyBrandsCollectionResponse>>(new GetWarrantyBrandsRequest(), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    [Produces(typeof(WarrantyBrandResponse))]
    public async Task<IActionResult> CreateWarrantyBrand(
        [FromForm] CreateWarrantyBrandRequest request,
        IValidator<CreateWarrantyBrandRequest> validator,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        Result<WarrantyBrandResponse> result = await bus.InvokeAsync<Result<WarrantyBrandResponse>>(request, cancellationToken);
        if (result.IsFailure)
        {
            return result.ToActionResult();
        }

        return CreatedAtAction(nameof(GetWarrantyBrandById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWarrantyBrand(string id, CancellationToken cancellationToken)
    {
        Result result = await bus.InvokeAsync<Result>(new DeleteWarrantyBrandRequest(id), cancellationToken);
        return result.ToActionResult();
    }
 }
