using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.CreateExciseNomenclature;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.UpdateExciseNomenclature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.CnCodes;

[Authorize]
[ApiController]
[Route("api/excise/cn-codes")]
public sealed class CnCodesController :
    ExciseNomenclatureController<CnCode, CreateCnCodeNomenclatureRequest, UpdateCnCodeNomenclatureRequest>
{
    public CnCodesController(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    protected override string EntityIdPrefix => "cn";
}
