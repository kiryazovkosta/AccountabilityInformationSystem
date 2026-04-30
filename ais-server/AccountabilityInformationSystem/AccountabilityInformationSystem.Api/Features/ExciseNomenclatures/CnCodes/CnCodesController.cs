using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.CnCodes.Create;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.CnCodes;

[Authorize]
[ApiController]
[Route("api/excise/cn-codes")]
public sealed class CnCodesController
    : ExciseNomenclatureController<CnCode, CreateCnCodeNomenclatureRequest, UpdateCnCodeNomenclatureRequest>
{
    protected override string EntityIdPrefix => "cn";
}
