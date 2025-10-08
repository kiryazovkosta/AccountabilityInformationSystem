using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Entities;
using AccountabilityInformationSystem.Api.Entities.Flow;
using AccountabilityInformationSystem.Api.Models.Flow.Ikunks;
using AccountabilityInformationSystem.Api.Models.Warehouses;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Controllers.Flow;

[ApiController]
[Route("api/flow/ikunks")]
public sealed class IkunksController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IkunksCollectionResponse>> GetIkunks(CancellationToken cancellationToken)
    {
        List<IkunkResponse> ikunksResponse = await dbContext
            .Ikunks
            .Include(ikunk => ikunk.Warehouse)
            .Include(ikunk => ikunk.MeasurementPoints)
            .AsNoTracking()
            .OrderBy(ikunk => ikunk.OrderPosition)
            .Select(IkunkQueries.ProjectToResponse())
            .ToListAsync(cancellationToken);
        return Ok(new IkunksCollectionResponse() { Items = ikunksResponse });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IkunkResponse>> GetIkunkById(
        string id, 
        CancellationToken cancellationToken)
    {
        IkunkResponse? ikunkResponse = await dbContext
            .Ikunks
            .AsNoTracking()
            .Include(ikunk => ikunk.Warehouse)
            .Include(ikunk => ikunk.MeasurementPoints.Where(mp => !mp.IsDeleted))
            .Select(IkunkQueries.ProjectToResponse())
            .FirstOrDefaultAsync(ikunk => ikunk.Id == id, cancellationToken);
        if (ikunkResponse is null)
        {
            return NotFound();
        }
        return Ok(ikunkResponse);
    }

    [HttpPost]
    public async Task<ActionResult<IkunkResponse>> CreateIkunk(
        [FromBody] CreateIkunkRequest request,
        IValidator<CreateIkunkRequest> validator,
        CancellationToken cancellationToken)
    {
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

        Ikunk ikunk = request.ToEntity();
        await dbContext.Ikunks.AddAsync(ikunk, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        IkunkResponse ikunkResponse = ikunk.ToResponse();
        return CreatedAtAction(
            nameof(GetIkunkById), 
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

        ikunk.UpdateFromRequest(request);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
