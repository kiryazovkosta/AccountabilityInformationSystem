using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Create;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.ApCodes.Create;

public sealed class CreateApCodeNomenclatureRequestHandler(
    UserContext userContext,
    ApplicationDbContext dbContext)
    : CreateExciseNomenclatureRequestHandler<ApCode, CreateApCodeNomenclatureRequest>(userContext, dbContext);
