using System.Text;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.ResetPassword;

public sealed class ResetPasswordRequestHandler(UserManager<IdentityUser> userManager)
{
    public async Task<Result> Handle(ResetPasswordRequest request, CancellationToken _)
    {
        IdentityUser? identityUser = await userManager.FindByIdAsync(request.UserId);
        if (identityUser is null)
        {
            return Result.Failure(
                new Error("user", "Invalid password reset request."),
                ResultFailureType.BadRequest);
        }

        string token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
        IdentityResult result = await userManager.ResetPasswordAsync(identityUser, token, request.NewPassword);
        if (!result.Succeeded)
        {
            IReadOnlyList<Error> errors = [..result.Errors.Select(e => new Error(e.Code, e.Description))];
            return Result.Failure(errors, ResultFailureType.BadRequest);
        }

        return Result.Success(ResultSuccessType.NoContent);
    }
}
