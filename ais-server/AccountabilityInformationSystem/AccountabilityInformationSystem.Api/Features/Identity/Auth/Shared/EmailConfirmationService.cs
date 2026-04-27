using System.Text;
using System.Text.Encodings.Web;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;

public sealed class EmailConfirmationService(
    UserManager<IdentityUser> userManager,
    IEmailSender emailSender,
    IOptions<FrontendOptions> frontendOptions)
{
    private readonly FrontendOptions _frontendOptions = frontendOptions.Value;

    public async Task<Result> SendConfirmationEmailAsync(
        IdentityUser identityUser,
        User user)
    {
        string code = await userManager.GenerateEmailConfirmationTokenAsync(identityUser);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        string callbackUrl = $"{_frontendOptions.HostName}{_frontendOptions.ConfirmEmail}?userId={identityUser.Id}&code={code}";
        string encodedCallbackUrl = HtmlEncoder.Default.Encode(callbackUrl);

        string message = @$"Hello {user.FirstName},<br/><br/>
Thank you for registering with the Accountability Information System with username:<strong>{user.Username}</strong>.<br/>
Please confirm your email address by clicking the following <strong><a href='{encodedCallbackUrl}'>link</a></strong><br/><br/>
If you did not create this account, you can safely ignore this email.<br/><br/>Regards,<br/>
The AIS Team";

        try
        {
            await emailSender.SendEmailAsync(identityUser.Email!, "AIS Registration confirmation", message);
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("smtp", ex.ToString()), ResultFailureType.BadRequest);
        }

        return Result.Success();
    }
}
