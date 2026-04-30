using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Update;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.ApCodes.Update;

public sealed class UpdateApCodeNomenclatureRequestHandler(
    UserContext userContext,
    ApplicationDbContext dbContext)
    : UpdateExciseNomenclatureRequestHandler<ApCode>(userContext, dbContext);
