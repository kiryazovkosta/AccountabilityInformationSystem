using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Create;

public class CreateExciseNomenclatureRequestHandler<TEntity, TCreateRequest>(
    UserContext userContext,
    ApplicationDbContext dbContext)
    where TEntity : AuditableEntity, IEntity, IExciseEntity, new()
    where TCreateRequest : CreateExciseNomenclatureRequest
{
    public async Task<Result<ExciseNomenclatureResponse>> Handle(
        CreateExciseNomenclatureCommand<TEntity, TCreateRequest> command,
        CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Result<ExciseNomenclatureResponse>.Failure(
                new Error("User", "Unauthorized"),
                ResultFailureType.Unauthorized);
        }

        bool codeExists = await dbContext
            .Set<TEntity>()
            .AnyAsync(en => en.Code == command.Request.Code, cancellationToken);
        if (codeExists)
        {
            return Result<ExciseNomenclatureResponse>.Failure(
                new Error("Code", $"{typeof(TEntity).Name} with specific code already exists!"),
                ResultFailureType.Conflict);
        }

        TEntity entity = command.Request.ToEntity<TEntity>(user.Email, command.EntityIdPrefix);
        await dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<ExciseNomenclatureResponse>.Success(entity.ToResponse());
    }
}
