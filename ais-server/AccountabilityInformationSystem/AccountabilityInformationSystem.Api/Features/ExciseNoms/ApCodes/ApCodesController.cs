using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.ApCodes.Create;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.ApCodes.Update;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.ApCodes;

[Authorize]
[ApiController]
[Route("api/excise/ap-codes")]
public sealed class ApCodesController
    : ExciseNomenclatureController<ApCode, CreateApCodeNomenclatureRequest, UpdateApCodeNomenclatureRequest>
{
    protected override UpdateApCodeNomenclatureRequest WithId(UpdateApCodeNomenclatureRequest request, string id) =>
        request with { Id = id };
}
