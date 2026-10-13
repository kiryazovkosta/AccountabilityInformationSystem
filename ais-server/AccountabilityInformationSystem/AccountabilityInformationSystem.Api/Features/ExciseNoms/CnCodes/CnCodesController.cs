using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.CnCodes.Create;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.CnCodes.Update;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.CnCodes;

[Authorize]
[ApiController]
[Route("api/excise/cn-codes")]
public sealed class CnCodesController
    : ExciseNomenclatureController<CnCode, CreateCnCodeNomenclatureRequest, UpdateCnCodeNomenclatureRequest>
{
    protected override UpdateCnCodeNomenclatureRequest WithId(UpdateCnCodeNomenclatureRequest request, string id) =>
        request with { Id = id };
}
