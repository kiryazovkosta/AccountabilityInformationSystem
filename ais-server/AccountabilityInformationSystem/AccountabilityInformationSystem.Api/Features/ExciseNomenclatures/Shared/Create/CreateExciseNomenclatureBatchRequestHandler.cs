using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Create;

public class CreateExciseNomenclatureBatchRequestHandler<TEntity, TCreateRequest>(
    UserContext userContext,
    ApplicationDbContext dbContext)
    where TEntity : AuditableEntity, IEntity, IExciseEntity, new()
    where TCreateRequest : CreateExciseNomenclatureRequest
{
    public async Task<Result> Handle(
        CreateExciseNomenclatureBatchCommand<TEntity, TCreateRequest> command,
        CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Result.Failure(
                new Error("User", "Unauthorized"),
                ResultFailureType.Unauthorized);
        }

        HashSet<string> codes = [.. command.Entries.Select(e => e.Code)];
        if (codes.Count != command.Entries.Count)
        {
            return Result.Failure(
                new Error("Entries", "Invalid number of unique codes"),
                ResultFailureType.BadRequest);
        }

        bool alreadyExists = await dbContext
            .Set<TEntity>()
            .AnyAsync(e => codes.Contains(e.Code), cancellationToken);
        if (alreadyExists)
        {
            return Result.Failure(
                new Error("Entries", "One or more entities with provided codes already exists"),
                ResultFailureType.Conflict);
        }

        List<TEntity> entities = [.. command.Entries.Select(e => e.ToEntity<TEntity>(user.Email, command.EntityIdPrefix))];
        await dbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ResultSuccessType.Created);
    }
}
