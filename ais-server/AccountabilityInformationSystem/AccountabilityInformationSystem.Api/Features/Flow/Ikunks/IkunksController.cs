using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Create;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.GetAll;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.GetById;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Update;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace AccountabilityInformationSystem.Api.Features.Flow.Ikunks;

[ApiController]
[Route("api/flow/ikunks")]
[ApiVersion(1.0)]
[Authorize(Roles = $"{Role.Admin},{Role.FlowUser}")]
public sealed class IkunksController(IMessageBus bus) : ApiController
{
    [HttpGet]
    [Produces(typeof(PaginationResponse<IkunkResponse>))]
    public async Task<IActionResult> GetIkunks(
        [FromQuery] GetIkunksRequest request,
        CancellationToken cancellationToken)
    {
        Result<PaginationResponse<ExpandoObject>> result =
            await bus.InvokeAsync<Result<PaginationResponse<ExpandoObject>>>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> GetIkunk(
        string id,
        string? fields,
        CancellationToken cancellationToken)
    {
        GetIkunkRequest request = new(id, fields);
        Result<ExpandoObject> result = await bus.InvokeAsync<Result<ExpandoObject>>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    [ApiVersion(2.0)]
    public async Task<IActionResult> GetIkunkV2(
        string id,
        string? fields,
        CancellationToken cancellationToken)
    {
        GetIkunkV2Request request = new(id, fields);
        Result<ExpandoObject> result = await bus.InvokeAsync<Result<ExpandoObject>>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateIkunk(
        [FromBody] CreateIkunkRequest request,
        IValidator<CreateIkunkRequest> validator,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        Result<IkunkResponse> result = await bus.InvokeAsync<Result<IkunkResponse>>(request, cancellationToken);
        if (result.IsFailure)
        {
            return result.ToActionResult();
        }

        return CreatedAtAction(
            nameof(GetIkunk),
            new { id = result.Value!.Id },
            result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIkunk(
        string id,
        [FromBody] UpdateIkunkRequest request,
        IValidator<UpdateIkunkRequest> validator,
        CancellationToken cancellationToken)
    {
        request = request with { Id = id };
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        Result result = await bus.InvokeAsync<Result>(request, cancellationToken);
        return result.ToActionResult();
    }
}
