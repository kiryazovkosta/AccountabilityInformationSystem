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
        var (host, port) = ResolveSmtpEndpoint();

        using MimeMessage message = new();
        message.From.Add(MailboxAddress.Parse(smtp.From));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;
        BodyBuilder builder = new(){ HtmlBody = htmlMessage };
        message.Body = builder.ToMessageBody();

        using SmtpClient client = new SmtpClient();
        await client.ConnectAsync(host, port, SecureSocketOptions.None);

        if (!string.IsNullOrEmpty(smtp.Username))
        {
            await client.AuthenticateAsync(smtp.Username, smtp.Password);
        }

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    private (string host, int port) ResolveSmtpEndpoint()
    {
        string? connectionString = configuration.GetConnectionString("mailpit");
        if (!string.IsNullOrEmpty(connectionString))
        {
            var csb = new DbConnectionStringBuilder { ConnectionString = connectionString };
            if (csb.TryGetValue("endpoint", out object val))
            {
                Uri uri = new(val.ToString()!);
                return (uri.Host, uri.Port);
            }
        }

        return (options.Value.Host, options.Value.Port);
    }
}
