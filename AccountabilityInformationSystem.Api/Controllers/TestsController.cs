using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Controllers;

[ApiController]
[Route("api/tests")]
public sealed class TestsController : ControllerBase
{
    [HttpGet("ping")]
    public ActionResult<string> Ping()
    {
        throw new IndexOutOfRangeException("Test exception for demonstration purposes.");

        //return Ok("pong");
    }
}
