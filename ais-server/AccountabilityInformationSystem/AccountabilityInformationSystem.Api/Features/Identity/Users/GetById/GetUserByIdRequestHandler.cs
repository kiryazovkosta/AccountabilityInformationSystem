using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Identity.Users.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Identity.Users.GetById;

public sealed class GetUserByIdRequestHandler(ApplicationDbContext dbContext)
{
    public async Task<Result<UserResponse>> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        UserResponse? userResponse = await dbContext.Users
            .AsNoTracking()
            .Select(UserQueries.ProjectToResponse())
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (userResponse is null)
        {
            return Result<UserResponse>.Failure(
                new Error("Id", "User with provided Id does not exist."),
                ResultFailureType.NotFound);
        }

        return Result<UserResponse>.Success(userResponse);
    }
}
