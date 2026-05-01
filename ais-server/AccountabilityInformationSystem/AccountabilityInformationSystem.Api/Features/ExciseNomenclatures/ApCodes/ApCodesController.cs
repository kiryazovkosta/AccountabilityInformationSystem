using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.ApCodes.Create;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.ApCodes;

[Authorize]
[ApiController]
[Route("api/excise/ap-codes")]
public sealed class ApCodesController
    : ExciseNomenclatureController<ApCode, CreateApCodeNomenclatureRequest, UpdateApCodeNomenclatureRequest>
{
    protected override string EntityIdPrefix => "ap";
}
