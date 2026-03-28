using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Features.Tests;

[Route("api/tests")]
[ApiController]
public sealed class TestsController(IEmailSender sender) : ControllerBase
{
    [HttpGet("send-email")]
    public async Task<IActionResult> SendTestMessage()
    {
        await sender.SendEmailAsync("test@example.com", "Test", "Test message from MailKit.");
        return Ok("Email sent");
    }
}
