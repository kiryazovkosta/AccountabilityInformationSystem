using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Create;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Delete;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.GetAll;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.GetById;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Shared;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.GetAll;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.GetById;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords;

[ApiController]
[Route("api/family/warranty-records")]
[ApiVersion("1.0")]
[Authorize]
public sealed class WarrantyRecordsController(IMessageBus bus) : ApiController
{
    [HttpGet]
    [Produces(typeof(PaginationResponse<WarrantyRecordListResponse>))]
    public async Task<IActionResult> GetWarrantyRecords(
        [FromQuery] GetWarrantyRecordsRequest request,
        CancellationToken cancellationToken)
    {
        Result<PaginationResponse<ExpandoObject>> result =
            await bus.InvokeAsync<Result<PaginationResponse<ExpandoObject>>>(request, cancellationToken);
        return result.ToActionResult();
    }

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
    public async Task<IActionResult> GetWarrantyRecordById(
        string id,
        string? fields,
        CancellationToken cancellationToken)
    {
        GetWarrantyRecordRequest request = new(id, fields);
        Result<ExpandoObject> result = await bus.InvokeAsync<Result<ExpandoObject>>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWarrantyRecord(string id, CancellationToken cancellationToken)
    {
        Result result = await bus.InvokeAsync<Result>(new DeleteWarrantyRecordRequest(id), cancellationToken);
        return result.ToActionResult();
    }
}
