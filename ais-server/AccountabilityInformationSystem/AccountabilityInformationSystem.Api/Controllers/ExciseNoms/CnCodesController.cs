using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Entities.Excise;
using AccountabilityInformationSystem.Api.Models.ExciseNomenclatures;
using AccountabilityInformationSystem.Api.Models.ExciseNomenclatures.ApCodes;
using AccountabilityInformationSystem.Api.Services.UserContexting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Controllers.ExciseNoms;

[Authorize]
[ApiController]
[Route("api/excise/cn-codes")]
public sealed class CnCodesController : ExciseNomenclatureController<CnCode, CreateCnCodeNomenclatureRequest>
{
    public CnCodesController(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    protected override string EntityIdPrefix => "cn";
}
