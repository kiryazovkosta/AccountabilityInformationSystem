using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AccountabilityInformationSystem.Api.Shared.Services.UserContexting;

public sealed class UserContext (
    IHttpContextAccessor httpContextAccessor,
    ApplicationDbContext applicationDbContext,
    IMemoryCache memoryCache)
{
    private const string CacheKeyPrefix = "users:id:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public async Task<User?> GetUserAsync(CancellationToken cancellationToken = default)
    {
        string? identityId = httpContextAccessor.HttpContext?.User.GetIdentityId();
        if (string.IsNullOrWhiteSpace(identityId))
        {
            return null;
        }

        string cacheKey = $"{CacheKeyPrefix}{identityId}";

        User? user = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetAbsoluteExpiration(CacheDuration);
            User? user = await applicationDbContext.Users
                .AsNoTracking()
                .Where(user => user.IdentityId == identityId)
                .FirstOrDefaultAsync(cancellationToken);

            return user;
        });

        return user;
    }
}
