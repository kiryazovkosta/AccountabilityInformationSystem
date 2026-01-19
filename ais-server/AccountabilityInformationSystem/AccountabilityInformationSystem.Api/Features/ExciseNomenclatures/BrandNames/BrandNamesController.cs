using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.CreateExciseNomenclature;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.UpdateExciseNomenclature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.BrandNames;

[Authorize]
[ApiController]
[Route("api/excise/brand-names")]
public sealed class BrandNamesController
    : ExciseNomenclatureController<BrandName, CreateBrandNameNomenclatureRequest, UpdateBrandNameNomenclatureRequest>
{
    public BrandNamesController(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    protected override string EntityIdPrefix => "bn";
}
