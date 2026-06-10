using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Create;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Shared;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords;

[ApiController]
[Route("api/family/warranty-records")]
[Authorize]
public sealed class WarrantyRecordsController(IMessageBus bus) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateWarrantyRecord(
        [FromForm] CreateWarrantyRecordRequest request,
        IValidator<CreateWarrantyRecordRequest> validator,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        Result<WarrantyRecordResponse> result = await bus.InvokeAsync<Result<WarrantyRecordResponse>>(request, cancellationToken);
        if (result.IsFailure)
        {
            return result.ToActionResult();
        }

        return CreatedAtAction(nameof(GetWarrantyRecordById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpGet("{id}")]
    public Task<IActionResult> GetWarrantyRecordById(string id, CancellationToken cancellationToken)
    {
        _ = id;
        _ = cancellationToken;
        return Task.FromResult<IActionResult>(NotFound());
    }
}
