using Microsoft.AspNetCore.Identity.UI.Services;

namespace AccountabilityInformationSystem.IntegrationTests.Infrastructure.Stubs;

internal sealed class NullEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage) =>
        Task.CompletedTask;
}
