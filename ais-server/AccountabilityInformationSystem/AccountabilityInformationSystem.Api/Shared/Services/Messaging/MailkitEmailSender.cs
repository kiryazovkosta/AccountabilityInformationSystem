using System.Data.Common;
using AccountabilityInformationSystem.Api.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AccountabilityInformationSystem.Api.Shared.Services.Messaging;

public class MailKitEmailSender(
    IOptions<SmtpOptions> options,
    IConfiguration configuration) : IEmailSender
{
    private readonly SmtpOptions smtp = options.Value;
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        SmtpConfig smtpConfig = ResolveSmtpEndpoint();

        using MimeMessage message = new();
        message.From.Add(MailboxAddress.Parse(smtp.From));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;
        BodyBuilder builder = new(){ HtmlBody = htmlMessage };
        message.Body = builder.ToMessageBody();

        using SmtpClient client = new();
        await client.ConnectAsync(smtpConfig.Host, smtpConfig.Port, SecureSocketOptions.None);

        if (!string.IsNullOrEmpty(smtp.Username) && !string.IsNullOrEmpty(smtp.Password))
        {
            await client.AuthenticateAsync(smtp.Username, smtp.Password);
        }

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    private SmtpConfig ResolveSmtpEndpoint()
    {
        string? connectionString = configuration.GetConnectionString("mailpit");
        if (!string.IsNullOrEmpty(connectionString))
        {
            DbConnectionStringBuilder csb = new()  { ConnectionString = connectionString };
            if (csb.TryGetValue("endpoint", out object val))
            {
                Uri uri = new(val.ToString()!);
                return new SmtpConfig(uri.Host, uri.Port);
            }
        }

        return new SmtpConfig(options.Value.Host, options.Value.Port);
    }

    private sealed record SmtpConfig(string Host, int Port);
}
