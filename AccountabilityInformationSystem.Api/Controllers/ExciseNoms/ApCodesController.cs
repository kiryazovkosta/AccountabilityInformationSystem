using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Entities.Excise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Controllers.ExciseNoms;

[Authorize]
[ApiController]
[Route("api/excise/ap-codes")]
public sealed class ApCodesController : ExciseNomenclatureController<ApCode>
{
    public ApCodesController(ApplicationDbContext dbContext) 
        : base(dbContext)
    {
    }
}
