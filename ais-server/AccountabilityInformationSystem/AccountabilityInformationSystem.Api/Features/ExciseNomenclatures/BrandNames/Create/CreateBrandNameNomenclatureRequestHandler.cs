using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Create;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.BrandNames.Create;

public sealed class CreateBrandNameNomenclatureRequestHandler(
    UserContext userContext,
    ApplicationDbContext dbContext)
    : CreateExciseNomenclatureRequestHandler<BrandName, CreateBrandNameNomenclatureRequest>(userContext, dbContext);
