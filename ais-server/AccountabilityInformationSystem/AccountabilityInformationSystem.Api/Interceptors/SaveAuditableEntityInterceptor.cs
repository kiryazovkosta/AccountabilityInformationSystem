using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.Api.Shared.Services.CurrentUserAccessing;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AccountabilityInformationSystem.Api.Interceptors;

public class SaveAuditableEntityInterceptor(
    TimeProvider timeProvider,
    CurrentUserAccessor currentUserAccessor) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            List<EntityEntry<AuditableEntity>> entities = [.. eventData.Context.ChangeTracker.Entries<AuditableEntity>()];
            string userName = currentUserAccessor.GetCurrentUser().UserName ?? ApplicationConstants.DefaultUserName;

            foreach (EntityEntry<AuditableEntity> entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    entity.Property(x => x.CreatedAt).CurrentValue = timeProvider.GetUtcNow().UtcDateTime;
                    entity.Property(e => e.CreatedBy).CurrentValue = userName;
                }

                if (entity.State == EntityState.Modified)
                {
                    entity.Property(x => x.ModifiedAt).CurrentValue = timeProvider.GetUtcNow().UtcDateTime;
                    entity.Property(e => e.ModifiedBy).CurrentValue = userName;
                }
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
