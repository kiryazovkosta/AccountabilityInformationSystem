using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.CreateExciseNomenclature;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.UpdateExciseNomenclature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.ApCodes;

[Authorize]
[ApiController]
[Route("api/excise/ap-codes")]
public sealed class ApCodesController
    : ExciseNomenclatureController<ApCode, CreateApCodeNomenclatureRequest, UpdateApCodeNomenclatureRequest>
{
    public ApCodesController(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    protected override string EntityIdPrefix => "ap";
}
