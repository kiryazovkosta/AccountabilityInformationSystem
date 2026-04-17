using System.Text;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.ConfirmEmail;

public class ConfirmEmailRequestHandler(
    UserManager<IdentityUser> userManager,
    ApplicationDbContext applicationDbContext,
    IDataProtectionProvider dataProtectionProvider
    )
{
    private readonly ITimeLimitedDataProtector _setupProtector =
        dataProtectionProvider.CreateProtector("TwoFactorSetupToken").ToTimeLimitedDataProtector();

    public async Task<Result<ConfirmEmailResponse>> Handle(ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Code))
        {
            return Result<ConfirmEmailResponse>.Failure(
                new Error("UserId or Code", "Invalid user identifier or code!"));
        }

        IdentityUser identityUser = await userManager.FindByIdAsync(request.UserId);
        if (identityUser is null)
        {
            return Result<ConfirmEmailResponse>.Failure(
                new Error("UserId", $"Unable to load user with ID '{request.UserId}'."),
                ResultFailureType.NotFound);
        }

        User? user = await applicationDbContext.Users
            .FirstOrDefaultAsync(u => u.IdentityId == identityUser.Id, cancellationToken);
        if (user is null)
        {
            return Result<ConfirmEmailResponse>.Failure(
                new Error("UserId", $"Unable to load user with ID '{request.UserId}'."),
                ResultFailureType.NotFound);
        }

        string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
        IdentityResult identityResult = await userManager.ConfirmEmailAsync(identityUser, code);
        if (!identityResult.Succeeded)
        {
            IReadOnlyList<Error> errors = identityResult.Errors
                .Select(e => new Error(e.Code, e.Description))
                .ToList();
            return Result<ConfirmEmailResponse>.Failure(errors, ResultFailureType.BadRequest);
        }

        // Check if 2FA setup is required
        if (user.Enable2Fa == true && !identityUser.TwoFactorEnabled)
        {
            string setupToken = _setupProtector.Protect(identityUser.Id, TimeSpan.FromMinutes(10));
            return Result<ConfirmEmailResponse>.Success(new ConfirmEmailResponse(true, setupToken));
        }

        return Result<ConfirmEmailResponse>.Success(new ConfirmEmailResponse(false));
    }
}
