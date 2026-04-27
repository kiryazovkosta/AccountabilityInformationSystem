using System.Data.Common;
using AccountabilityInformationSystem.Api.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AccountabilityInformationSystem.Api.Shared.Services.Messaging;

public class BrevoSmtpEmailSender(
    IOptions<SmtpOptions> options) : IEmailSender
{
    private readonly SmtpOptions smtp = options.Value;
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using MimeMessage message = new();
        message.From.Add(MailboxAddress.Parse(smtp.From));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;
        BodyBuilder builder = new() { HtmlBody = htmlMessage };
        message.Body = builder.ToMessageBody();

        using SmtpClient client = new();
        await client.ConnectAsync(smtp.Host, smtp.Port, SecureSocketOptions.StartTls);

        if (!string.IsNullOrEmpty(smtp.Username) && !string.IsNullOrEmpty(smtp.Password))
        {
            await client.AuthenticateAsync(smtp.Username, smtp.Password);
        }

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
