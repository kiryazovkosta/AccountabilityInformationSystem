using System.Linq.Dynamic.Core.Tokenizer;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Refresh;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;
using AccountabilityInformationSystem.Api.Features.Identity.Users.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Settings;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Services.Tokenizing;
using Mapster;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor;
using QRCoder;
using AccountabilityInformationSystem.Api.Shared.Extensions;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth;

[Route("api/identity/auth")]
public sealed class AuthController(
    UserManager<IdentityUser> userManager,
    ApplicationIdentityDbContext identityDbContext,
    ApplicationDbContext applicationDbContext,
    TokenProvider tokenProvider,
    IAntiforgery antiforgery,
    IEmailSender emailSender,
    IOptions<JwtAuthOptions> options,
    IOptions<FrontendOptions> frontendOptions,
    IDataProtectionProvider dataProtectionProvider) : ApiController
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;
    private readonly FrontendOptions _frontendOptions = frontendOptions.Value;
    private readonly ITimeLimitedDataProtector _setupProtector =
        dataProtectionProvider.CreateProtector("TwoFactorSetupToken").ToTimeLimitedDataProtector();

    [HttpPost("register")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Register(
        RegisterUserRequest registerRequest,
        CancellationToken cancellationToken)
    {
        IActionResult? registerResult = 
            await identityDbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            await using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync(cancellationToken);
            applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
            await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

            IdentityUser identityUser = new()
            {
                UserName = registerRequest.Username,
                Email = registerRequest.Email,
            };

            IdentityResult identityResult = await userManager.CreateAsync(identityUser, registerRequest.Password);
            if (!identityResult.Succeeded)
            {
                return IdentityProblem("Unable to register user, please try again!", identityResult.Errors);
            }

            identityResult = await userManager.AddToRoleAsync(identityUser, Role.Member);
            if (!identityResult.Succeeded)
            {
                return IdentityProblem("Unable to register user, please try again!", identityResult.Errors);
            }

            User user = registerRequest.ToEntity();
            user.IdentityId = identityUser.Id;

            await applicationDbContext.Users.AddAsync(user, cancellationToken);
            await applicationDbContext.SaveChangesAsync(cancellationToken);

            await identityDbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            string code = await userManager.GenerateEmailConfirmationTokenAsync(identityUser);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            string callbackUrl = $"{_frontendOptions.HostName}{_frontendOptions.ConfirmEmail}?userId={identityUser.Id}&code={code}";
            string encodedcallbackUrl = HtmlEncoder.Default.Encode(callbackUrl);

            string message = @$"Hello {user.FirstName},<br/><br/>
Thank you for registering with the Accountability Information System.<br/>
Please confirm your email address by clicking the following <a href='{encodedcallbackUrl}'>link</a><br/><br/>
If you did not create this account, you can safely ignore this email.<br/><br/>Regards,<br/>
The AIS Team";
            await emailSender.SendEmailAsync(identityUser.Email!, "AIS Registration confirmation", message);
            return Created();
        });

        return registerResult ?? Problem(
            detail: "Unable to register user, please try again!",
            statusCode: StatusCodes.Status400BadRequest
        );
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Login(/*LoginUserRequest request, CancellationToken cancellationToken*/)
    {
        //Result<LoginUserResponse> result = await loginHandler.Handle(request, cancellationToken);
        //if (result.IsFailure)
        //{
        //    return ErrorResponseFactory.Create(result.Error);
        //}

        //if (result.IsSuccess && result.Value is not null && result.Value.RequiresTwoFactorSetup)
        //{
        //    return StatusCode(StatusCodes.Status202Accepted, result.Value);
        //}

        //SetTokensInsideCookies(new AccessTokenResponse(result.Value.AccessToken!, result.Value.RefreshToken!), HttpContext);
        //SetAntiforgeryToken();
        return Ok();
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out string? refreshTokenValue) 
            || string.IsNullOrWhiteSpace(refreshTokenValue))
        {
            return Unauthorized("Refresh token missing.");
        }

        RefreshToken? storedToken = await identityDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenValue, cancellationToken);

        if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
        {
            return Unauthorized("Token expired or invalid.");
        }

        IList<string> roles = await userManager.GetRolesAsync(storedToken.User);
        AccessTokenRequest accessTokenRequest = new(
            storedToken.User.Id,
            storedToken.User.Email ?? string.Empty,
            roles);

        AccessTokenResponse response = tokenProvider.Create(accessTokenRequest);

        storedToken.Token = response.RefreshToken;
        storedToken.ExpiresAt = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays);
        identityDbContext.RefreshTokens.Update(storedToken);
        await identityDbContext.SaveChangesAsync(cancellationToken);

        SetTokensInsideCookies(response, HttpContext);
        SetAntiforgeryToken();

        return Ok();
    }

    [HttpGet("confirm-email", Name = "ConfirmEmailRoute")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> ConfirmEmail(string userId, string code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
        {
            return Problem(
                detail: "Invalid user identifier or code!",
                statusCode: StatusCodes.Status400BadRequest);
        }

        IdentityUser identityUser = await userManager.FindByIdAsync(userId);
        if (identityUser is null)
        {
            return Problem(
                detail: $"Unable to load user with ID '{userId}'.",
                statusCode: StatusCodes.Status404NotFound);
        }

        User? user = await applicationDbContext.Users
            .FirstOrDefaultAsync(u => u.IdentityId == identityUser.Id, cancellationToken);
        if (user is null)
        {
            return Problem(
                detail: $"Unable to load user with ID '{userId}'.",
                statusCode: StatusCodes.Status404NotFound);
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        IdentityResult identityResult = await userManager.ConfirmEmailAsync(identityUser, code);
        if (!identityResult.Succeeded)
        {
            return IdentityProblem("Unable to confirm email, please try again!", identityResult.Errors);
        }

        // Check if 2FA setup is required
        if (user.Enable2Fa == true && !identityUser.TwoFactorEnabled)
        {
            string setupToken = _setupProtector.Protect(identityUser.Id, TimeSpan.FromMinutes(10));
            return Ok(new { requiresTwoFactorSetup = true, setupToken });
        }

        return Ok(new { requiresTwoFactorSetup = false });
    }

    [HttpPost("2fa/setup")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> SetupTwoFactor(
        SetupTwoFactorRequest request)
    {
        string userId = _setupProtector.Unprotect(request.SetupToken);

        IdentityUser? identityUser = await userManager.FindByIdAsync(userId);
        if (identityUser is null)
        {
            return Problem(
                detail: "User not found.",
                statusCode: StatusCodes.Status404NotFound);
        }

        if (identityUser.TwoFactorEnabled)
        {
            return Problem(
                detail: "Two-factor authentication is already configured.",
                statusCode: StatusCodes.Status409Conflict);
        }

        await userManager.ResetAuthenticatorKeyAsync(identityUser);
        string? key = await userManager.GetAuthenticatorKeyAsync(identityUser);
        if (string.IsNullOrEmpty(key))
        {
            return Problem(
                detail: "Failed to return authentication key for the user",
                statusCode: StatusCodes.Status400BadRequest);
        }

        string otpauthUri =
            $"otpauth://totp/AIS:{identityUser.Email}?secret={key}&issuer=AIS&digits=6&algorithm=SHA1&period=30";

        using QRCodeGenerator qrGenerator = new();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(otpauthUri, QRCodeGenerator.ECCLevel.Q);
        using PngByteQRCode qrCode = new(qrCodeData);
        byte[] qrCodeBytes = qrCode.GetGraphic(20);
        string qrCodeBase64 = $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";

        return Ok(new SetupTwoFactorResponse(qrCodeBase64, key));
    }

    [HttpPost("2fa/verify")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> VerifyTwoFactor(
        VerifyTwoFactorRequest request)
    {
        string userId;
        try
        {
            userId = _setupProtector.Unprotect(request.SetupToken);
        }
        catch
        {
            return Problem(
                detail: "Invalid or expired setup token.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        IdentityUser? identityUser = await userManager.FindByIdAsync(userId);
        if (identityUser is null)
        {
            return Problem(
                detail: "User not found.",
                statusCode: StatusCodes.Status404NotFound);
        }

        bool isValid = await userManager.VerifyTwoFactorTokenAsync(
            identityUser,
            userManager.Options.Tokens.AuthenticatorTokenProvider,
            request.Code);

        if (!isValid)
        {
            return Problem(
                detail: "Invalid authenticator code.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        await userManager.SetTwoFactorEnabledAsync(identityUser, true);
           // Generate recovery codes (shown once — user must save them)
        IEnumerable<string>? recoveryCodes =
            await userManager.GenerateNewTwoFactorRecoveryCodesAsync(identityUser, 10);

        return Ok(new { recoveryCodes });
    }


    [HttpGet("csrf-token")]
    [AllowAnonymous]
    public IActionResult GetCsrfToken()
    {
        SetAntiforgeryToken();
        return NoContent();
    }

    [HttpPost("logout")]
    [Authorize]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult> Logout(CancellationToken cancellationToken)
    {
        if(HttpContext.Request.Cookies.TryGetValue("refreshToken", out string? refreshTokenValue))
        {
            RefreshToken? refreshToken = await identityDbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshTokenValue, cancellationToken);
            if (refreshToken is null || refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Unauthorized");
            }

            identityDbContext.RefreshTokens.Remove(refreshToken);
            await identityDbContext.SaveChangesAsync(cancellationToken);
        }

        SetAccessTokenCookie("accessToken", string.Empty, -1440, HttpContext);
        SetAccessTokenCookie("refreshToken", string.Empty, -1440, HttpContext);
        SetAccessTokenCookie("XSRF-TOKEN", string.Empty, -1440, HttpContext);

        return Ok(new { message = "Successfully logged out!" });
    }

    private void SetTokensInsideCookies(AccessTokenResponse token, HttpContext context)
    {
        SetAccessTokenCookie("accessToken", token.AccessToken, 30, context);
        SetAccessTokenCookie("refreshToken", token.RefreshToken, 10080, context);
    }

    private static void SetAccessTokenCookie(string name, string value, int minutesExpires, HttpContext context)
    {
        context.Response.Cookies.Append(name, value,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(minutesExpires),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });
    }

    private void SetAntiforgeryToken()
    {
        AntiforgeryTokenSet? tokens = antiforgery.GetAndStoreTokens(HttpContext);
        if (tokens is not null)
        {
            HttpContext.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken ?? string.Empty, new CookieOptions
            {
                HttpOnly = false,
                SameSite = SameSiteMode.None,
                Secure = true,
                IsEssential = true,
                Path = "/"
            });
        }
    }

}
