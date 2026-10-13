using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Update;

public abstract class UpdateExciseNomenclatureRequestHandler<TEntity, TUpdateRequest>(
    ApplicationDbContext dbContext)
        where TEntity : AuditableEntity, IEntity, IExciseEntity, new()
        where TUpdateRequest : UpdateExciseNomenclatureRequest
{
    public async Task<Result> Handle(
        TUpdateRequest request,
        CancellationToken cancellationToken)
    {
        TEntity? entity = await dbContext
            .Set<TEntity>()
            .FirstOrDefaultAsync(en => en.Id == request.Id, cancellationToken);
        if (entity is null)
        {
            return Result.Failure(
                new Error("Id", $"{typeof(TEntity).Name} with specific id does not exist!"),
                ResultFailureType.NotFound);
        }

        bool codeExists = await dbContext
            .Set<TEntity>()
            .AnyAsync(en => en.Code == request.Code && en.Id != entity.Id, cancellationToken);
        if (codeExists)
        {
            return Result.Failure(
                new Error("Code", $"{typeof(TEntity).Name} with specific code already exists!"),
                ResultFailureType.Conflict);
        }

        entity.UpdateFromRequest(request);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(ResultSuccessType.NoContent);
    }
}
