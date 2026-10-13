using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Update;
using AccountabilityInformationSystem.Api.Infrastructure.Data;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.BrandNames.Update;

public sealed class UpdateBrandNameNomenclatureRequestHandler(ApplicationDbContext dbContext)
    : UpdateExciseNomenclatureRequestHandler<BrandName, UpdateBrandNameNomenclatureRequest>(dbContext);
