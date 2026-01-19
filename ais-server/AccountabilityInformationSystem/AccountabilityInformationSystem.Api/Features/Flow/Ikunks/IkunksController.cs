using System.Dynamic;
using System.Linq.Dynamic.Core;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.CreateIkunk;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.UpdateIkunk;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Flow.Ikunks;

[ApiController]
[Route("api/flow/ikunks")]
[ApiVersion(1.0)]
[Authorize(Roles = $"{Role.Admin},{Role.FlowUser}")]
public sealed class IkunksController(ApplicationDbContext dbContext, UserContext userContext)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IkunksCollectionResponse>> GetIkunks(
        [FromQuery] QueryParameters query,
        SortMappingProvider sortMappingProvider,
        DataShapingService dataShapingService,
        CancellationToken cancellationToken)
    {
        if (!sortMappingProvider.ValidateMappings<IkunkResponse, Ikunk>(query.Sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid sort parameter. {query.Sort}");
        }

        if (!dataShapingService.Validate<IkunkResponse>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid fields parameter. {query.Fields}");
        }

        query.Search = query.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<IkunkResponse, Ikunk>();


        IQueryable<IkunkResponse> ikunksQuery = dbContext
            .Ikunks
            .Where(mp =>
                query.Search == null ||
                EF.Functions.Like(mp.Name, $"%{query.Search}%") ||
                EF.Functions.Like(mp.FullName, $"%{query.Search}%") ||
                mp.Description != null && EF.Functions.Like(mp.Description, $"%{query.Search}%")
            )
            .ApplySort(query.Sort, sortMappings, "OrderPosition")
            .AsNoTracking()
            .Select(IkunkQueries.ProjectToResponse());

        PaginationResponse<ExpandoObject> response = new()
        {
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = await ikunksQuery.CountAsync(cancellationToken),
            Items = dataShapingService.ShapeCollectionData(
                await ikunksQuery
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync(cancellationToken),
                query.Fields)
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    [MapToApiVersion(1.0)]
    public async Task<ActionResult<IkunkResponse>> GetIkunk(
        string id,
        string? fields,
        DataShapingService dataShapingService,
        CancellationToken cancellationToken)
    {
        if (!dataShapingService.Validate<IkunkResponse>(fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid fields parameter. {fields}");
        }

        IkunkResponse? ikunkResponse = await dbContext
            .Ikunks
            .AsNoTracking()
            .Select(IkunkQueries.ProjectToResponse())
            .FirstOrDefaultAsync(ikunk => ikunk.Id == id, cancellationToken);
        if (ikunkResponse is null)
        {
            return NotFound();
        }

        ExpandoObject shapedData = dataShapingService.ShapeData(ikunkResponse, fields);
        return Ok(shapedData);
    }

    [HttpGet("{id}")]
    [ApiVersion(2.0)]
    public async Task<ActionResult<IkunkResponse>> GetIkunkV2(
        string id,
        string? fields,
        DataShapingService dataShapingService,
        CancellationToken cancellationToken)
    {
        if (!dataShapingService.Validate<IkunkResponseV2>(fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid fields parameter. {fields}");
        }

        IkunkResponseV2? ikunkResponse = await dbContext
            .Ikunks
            .AsNoTracking()
            .Select(IkunkQueries.ProjectToResponseV2())
            .FirstOrDefaultAsync(ikunk => ikunk.Id == id, cancellationToken);
        if (ikunkResponse is null)
        {
            return NotFound();
        }

        ExpandoObject shapedData = dataShapingService.ShapeData(ikunkResponse, fields);
        return Ok(shapedData);
    }

    [HttpPost]
    public async Task<ActionResult<IkunkResponse>> CreateIkunk(
        [FromBody] CreateIkunkRequest request,
        IValidator<CreateIkunkRequest> validator,
        CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Unauthorized");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        if (!await dbContext.Warehouses.AnyAsync(w => w.Id == request.WarehouseId, cancellationToken))
        {
            return Problem(
                detail: "Warehouse with specific id does not exists!",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (await dbContext.Ikunks.AnyAsync(ik => ik.Name == request.Name, cancellationToken))
        {
            return Problem(
                detail: "Ikunk with specific name already exists!",
                statusCode: StatusCodes.Status409Conflict);
        }

        Ikunk ikunk = request.ToEntity(user.Email);
        await dbContext.Ikunks.AddAsync(ikunk, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        IkunkResponse ikunkResponse = ikunk.ToResponse();
        return CreatedAtAction(
            nameof(GetIkunk),
            new { id = ikunk.Id },
            ikunkResponse);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateIkunk(
        string id,
        [FromBody] UpdateIkunkRequest request,
        IValidator<UpdateIkunkRequest> validator,
        CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Unauthorized");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        bool nameExists = await dbContext
            .Ikunks
            .AnyAsync(ik => ik.Name == request.Name && ik.Id != id, cancellationToken);
        if (nameExists)
        {
            return Problem(
                detail: "Ikunk with specific name already exists!",
                statusCode: StatusCodes.Status409Conflict);
        }

        Ikunk? ikunk = await dbContext
            .Ikunks
            .FirstOrDefaultAsync(ik => ik.Id == id, cancellationToken);
        if (ikunk is null)
        {
            return Problem(detail: "Ikunk with specific id does not exists!",
                statusCode: StatusCodes.Status404NotFound);
        }

        if (request.WarehouseId is not null
            && !await dbContext.Warehouses.AnyAsync(w => w.Id == request.WarehouseId, cancellationToken) )
        {
            return Problem(
                detail: "Warehouse with specific id does not exists!",
                statusCode: StatusCodes.Status400BadRequest);
        }

        ikunk.UpdateFromRequest(request, user.Email);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
