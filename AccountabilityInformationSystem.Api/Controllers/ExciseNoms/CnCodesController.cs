using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Entities.Excise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Controllers.ExciseNoms;

[Authorize]
[ApiController]
[Route("api/excise/cn-codes")]
public sealed class CnCodesController : ExciseNomenclatureController<CnCode>
{
    public CnCodesController(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }
}
