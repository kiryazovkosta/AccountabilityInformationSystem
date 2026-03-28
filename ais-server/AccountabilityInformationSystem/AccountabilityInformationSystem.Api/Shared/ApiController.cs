using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Shared;

[ApiController]
public abstract class ApiController : ControllerBase
{
    protected IActionResult IdentityProblem(string detail, IEnumerable<IdentityError> errors)
    {
        Dictionary<string, object?> extensions = new()
        {
            { "errors", errors.ToDictionary(e => e.Code, e => e.Description) }
        };

        return Problem(
            detail: detail,
            statusCode: StatusCodes.Status400BadRequest,
            extensions: extensions
        );
    }
}
