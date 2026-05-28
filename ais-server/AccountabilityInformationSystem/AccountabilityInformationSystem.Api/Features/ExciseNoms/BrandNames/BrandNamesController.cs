using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.BrandNames.Create;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.BrandNames;

[Authorize]
[ApiController]
[Route("api/excise/brand-names")]
public sealed class BrandNamesController
    : ExciseNomenclatureController<BrandName, CreateBrandNameNomenclatureRequest, UpdateBrandNameNomenclatureRequest>
{
    protected override string EntityIdPrefix => "bn";
}
