using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.ToggleStatus;

public class ToggleExciseNomenclatureStatusRequestHandler<TEntity>(
    UserContext userContext,
    ApplicationDbContext dbContext)
    where TEntity : AuditableEntity, IEntity, IExciseEntity, new()
{
    public async Task<Result> Handle(
        ToggleExciseNomenclatureStatusCommand<TEntity> command,
        CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Result.Failure(
                new Error("User", "Unauthorized"),
                ResultFailureType.Unauthorized);
        }

        TEntity? entity = await dbContext
            .Set<TEntity>()
            .FirstOrDefaultAsync(en => en.Id == command.Id, cancellationToken);
        if (entity is null)
        {
            return Result.Failure(
                new Error("Id", $"{typeof(TEntity).Name} with specific id does not exist!"),
                ResultFailureType.NotFound);
        }

        entity.IsUsed = !entity.IsUsed;
        entity.ModifiedBy = user.Email;
        entity.ModifiedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ResultSuccessType.NoContent);
    }
}
