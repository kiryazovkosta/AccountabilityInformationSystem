using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Entities.Excise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Controllers.ExciseNoms;

[Authorize]
[ApiController]
[Route("api/excise/brand-names")]
public sealed class BrandNamesController : ExciseNomenclatureController<BrandName>
{
    public BrandNamesController(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }
}
