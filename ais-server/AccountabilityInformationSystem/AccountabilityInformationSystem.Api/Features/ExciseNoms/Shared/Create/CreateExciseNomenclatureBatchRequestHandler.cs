using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Create;

public abstract class CreateExciseNomenclatureBatchRequestHandler<TEntity, TCreateRequest>(
    ApplicationDbContext dbContext)
    where TEntity : AuditableEntity, IEntity, IExciseEntity, new()
    where TCreateRequest : CreateExciseNomenclatureRequest
{
    protected abstract string EntityIdPrefix { get; }

    public async Task<Result> Handle(
        CreateExciseNomenclatureBatchRequest<TCreateRequest> request,
        CancellationToken cancellationToken)
    {
        HashSet<string> codes = [.. request.Entries.Select(e => e.Code)];
        if (codes.Count != request.Entries.Count)
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

        List<TEntity> entities = [.. request.Entries.Select(e => e.ToEntity<TEntity>(EntityIdPrefix))];
        await dbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ResultSuccessType.Created);
    }
}
