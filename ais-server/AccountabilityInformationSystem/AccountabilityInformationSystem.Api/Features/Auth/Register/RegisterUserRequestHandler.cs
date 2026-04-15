using System;
using System.Text;
using System.Text.Encodings.Web;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;
using AccountabilityInformationSystem.Api.Features.Identity.Users.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace AccountabilityInformationSystem.Api.Features.Auth.Register;

public sealed class RegisterUserRequestHandler(
    ApplicationDbContext applicationDbContext,
    ApplicationIdentityDbContext identityDbContext,
    UserManager<IdentityUser> userManager,
    IEmailSender emailSender,
    IOptions<FrontendOptions> frontendOptions)
{
    private readonly FrontendOptions _frontendOptions = frontendOptions.Value;
    
    public async Task<Result> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        Result? registerResult = 
            await identityDbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            await using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync(cancellationToken);
            applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
            await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

            IdentityUser identityUser = new()
            {
                UserName = request.Username,
                Email = request.Email,
            };

            IdentityResult identityResult = await userManager.CreateAsync(identityUser, request.Password);
            if (!identityResult.Succeeded)
            {
                return Result.Failure(new Error("", "Unable to register user, please try again!"), ResultFailureType.BadRequest);
            }

            identityResult = await userManager.AddToRoleAsync(identityUser, Role.Member);
            if (!identityResult.Succeeded)
            {
                return Result.Failure(new Error("", "Unable to register user, please try again!"), ResultFailureType.BadRequest);
            }

            User user = request.ToEntity();
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
Thank you for registering with the Accountability Information System with username:<strong>{user.Username}</strong>.<br/>
Please confirm your email address by clicking the following <strong><a href='{encodedcallbackUrl}'>link</a></strong><br/><br/>
If you did not create this account, you can safely ignore this email.<br/><br/>Regards,<br/>
The AIS Team";
            await emailSender.SendEmailAsync(identityUser.Email!, "AIS Registration confirmation", message);
            return Result.Success(ResultSuccessType.Created);
        });

        return registerResult ?? Result.Failure(new Error("", "Unable to register user, please try again!"), ResultFailureType.BadRequest);
    }
}
