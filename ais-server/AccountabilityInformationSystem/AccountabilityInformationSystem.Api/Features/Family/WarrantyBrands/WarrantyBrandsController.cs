using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.GetAll;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.Shared;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands;

[ApiController]
[Route("api/family/warranty-brands")]
[Authorize]
public sealed class WarrantyBrandsController(IMessageBus bus) : ApiController
{
    [HttpGet]
    [Produces(typeof(WarrantyBrandsCollectionResponse))]
    public async Task<IActionResult> GetWarrantyBrands(CancellationToken cancellationToken)
    {
        Result<WarrantyBrandsCollectionResponse> result =
            await bus.InvokeAsync<Result<WarrantyBrandsCollectionResponse>>(new GetWarrantyBrandsRequest(), cancellationToken);
        return result.ToActionResult();
    }
}
