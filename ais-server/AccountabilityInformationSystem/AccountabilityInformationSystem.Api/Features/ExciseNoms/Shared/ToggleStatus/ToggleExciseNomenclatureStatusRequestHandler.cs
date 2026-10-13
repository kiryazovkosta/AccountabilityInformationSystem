using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.ToggleStatus;

public abstract class ToggleExciseNomenclatureStatusRequestHandler<TEntity>(
    ApplicationDbContext dbContext)
    where TEntity : AuditableEntity, IEntity, IExciseEntity, new()
{
    public async Task<Result> Handle(
        ToggleExciseNomenclatureStatusRequest<TEntity> request,
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

        entity.IsUsed = !entity.IsUsed;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ResultSuccessType.NoContent);
    }
}
