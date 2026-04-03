using System.Linq.Dynamic.Core.Tokenizer;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
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
    IOptions<FrontendOptions> frontendOptions) : ApiController
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;
    private readonly FrontendOptions _frontendOptions = frontendOptions.Value;

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
    public async Task<IActionResult> Login(LoginUserRequest request, CancellationToken cancellationToken)
    {
        IdentityUser identityUser = await userManager.FindByEmailAsync(request.Email);
        if (identityUser is null ||
            !await userManager.CheckPasswordAsync(identityUser, request.Password))
        {
            return Problem(
                detail: "Invalid username or password!",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        IEnumerable<string> roles = await userManager.GetRolesAsync(identityUser);

        AccessTokenRequest accessTokenRequest = new(identityUser.Id, identityUser.Email ?? string.Empty, roles);
        AccessTokenResponse response = tokenProvider.Create(accessTokenRequest);

        RefreshToken refreshToken = new()
        {
            Id = $"rt_{Guid.CreateVersion7()}",
            UserId = identityUser.Id,
            Token = response.RefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays)
        };

        await identityDbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);

        await identityDbContext.SaveChangesAsync(cancellationToken);

        SetTokensInsideCookies(response, HttpContext);
        SetAntiforgeryToken();

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
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
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

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        IdentityResult identityResult = await userManager.ConfirmEmailAsync(identityUser, code);
        if (!identityResult.Succeeded)
        {
            return IdentityProblem("Unable to confirm email, please try again!", identityResult.Errors);
        }

        return Ok("Thank you for confirming your email.");
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
