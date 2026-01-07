using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Entities.Excise;
using AccountabilityInformationSystem.Api.Entities.Identity;
using AccountabilityInformationSystem.Api.Models.ExciseNomenclatures;
using AccountabilityInformationSystem.Api.Services.UserContexting;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Controllers.ExciseNoms;

[Authorize]
[ApiController]
[Route("api/excise/ap-codes")]
public sealed class ApCodesController : ExciseNomenclatureController<ApCode, CreateApCodeNomenclatureRequest>
{
    public ApCodesController(ApplicationDbContext dbContext) 
        : base(dbContext)
    {
    }
}
