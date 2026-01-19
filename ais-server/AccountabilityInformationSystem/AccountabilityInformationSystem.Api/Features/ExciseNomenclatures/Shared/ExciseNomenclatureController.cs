using System.Dynamic;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.CreateExciseNomenclature;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.UpdateExciseNomenclature;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.GetExciseNomenclatures;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared;

[ResponseCache(Duration = 120)]
[Authorize]
[ApiController]
public abstract class ExciseNomenclatureController<TEntity, TCreateRequest, TUpdateRequest> : ControllerBase
    where TEntity : AuditableEntity, IEntity, IExciseEntity, new()
    where TCreateRequest: CreateExciseNomenclatureRequest
    where TUpdateRequest : UpdateExciseNomenclatureRequest
{
    protected readonly ApplicationDbContext DbContext;
    protected abstract string EntityIdPrefix { get; }

    protected ExciseNomenclatureController(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        DbContext = dbContext;
    }

    [HttpGet]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> GetAll(
        [FromQuery] ExciseNomenclatureQueryParameters query,
        [FromServices] SortMappingProvider sortMappingProvider,
        [FromServices] DataShapingService dataShapingService,
        CancellationToken cancellationToken)
    {
        if (!sortMappingProvider.ValidateMappings<ExciseNomenclatureResponse, TEntity>(query.Sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid sort parameter. {query.Sort}");
        }

        if (!dataShapingService.Validate<ExciseNomenclatureResponse>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid fields parameter. {query.Fields}");
        }

        query.Search = query.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<ExciseNomenclatureResponse, TEntity>();

        IQueryable<ExciseNomenclatureResponse> queryable = DbContext
            .Set<TEntity>()
            .Where(pt =>
                query.Search == null ||
                EF.Functions.Like(pt.BgDescription, $"%{query.Search}%") ||
                EF.Functions.Like(pt.DescriptionEn, $"%{query.Search}%")
            )
            .Where(en => query.IsUsed == null || en.IsUsed == query.IsUsed)
            .ApplySort(query.Sort, sortMappings)
            .AsNoTracking()
            .Select(ExciseNomenclatureQueries.ProjectToResponse<TEntity>());

        PaginationResponse<ExpandoObject> response = new()
        {
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = await queryable.CountAsync(cancellationToken),
            Items = dataShapingService.ShapeCollectionData(
                await queryable
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync(cancellationToken),
                query.Fields)
        };

        return Ok(response);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id,
        [FromQuery] FieldsOnlyQueryParameters query,
        [FromServices] DataShapingService dataShapingService,
        CancellationToken cancellationToken)
    {
        if (!dataShapingService.Validate<ExciseNomenclatureResponse>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid fields parameter. {query.Fields}");
        }

        ExciseNomenclatureResponse? exciseNomenclatureResponse = await DbContext
            .Set<TEntity>()
            .AsNoTracking()
            .Select(ExciseNomenclatureQueries.ProjectToResponse<TEntity>())
            .FirstOrDefaultAsync(mp => mp.Id == id, cancellationToken);
        if (exciseNomenclatureResponse is null)
        {
            return NotFound();
        }

        ExpandoObject shapedResponse = dataShapingService.ShapeData(exciseNomenclatureResponse, query.Fields);
        return Ok(shapedResponse);
    }

    [HttpPost]
    public async Task<ActionResult<ExciseNomenclatureResponse>> Create(
        [FromBody] TCreateRequest request,
        [FromServices] IValidator<TCreateRequest> validator,
        [FromServices] UserContext userContext,
        CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Unauthorized");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        if (await DbContext.Set<TEntity>().AnyAsync(en => en.Code == request.Code, cancellationToken))
        {
            return Problem(
                detail: $"{typeof(TEntity)} with specific code already exists!",
                statusCode: StatusCodes.Status409Conflict);
        }

        TEntity entity = request.ToEntity<TEntity>(user.Email, EntityIdPrefix);
        await DbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
        ExciseNomenclatureResponse response = entity.ToResponse();

        return CreatedAtAction(
            actionName: nameof(GetById),
            routeValues: new { id = response.Id },
            value: response);
    }

    [HttpPost("batch")]
    public async Task<ActionResult<List<ExciseNomenclatureResponse>>> CreateBatch(
        [FromBody] CreateExciseNomenclatureBatchRequest<TCreateRequest> request,
        [FromServices] IValidator<CreateExciseNomenclatureBatchRequest<TCreateRequest>> validator,
        [FromServices] UserContext userContext,
        CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Unauthorized");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        HashSet<string> createdCodes = [.. request.Entries.Select(e => e.Code)];
        if (createdCodes.Count != request.Entries.Count)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Invalid number of unique codes");
        }

        bool alreadyExists = await DbContext.Set<TEntity>().AnyAsync(e => createdCodes.Contains(e.Code), cancellationToken);
        if (alreadyExists)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, detail: "One or more entities with provided codes already exists");
        }

        List<TEntity> entities = [.. request.Entries.Select(en => en.ToEntity<TEntity>(user.Email, EntityIdPrefix))];
        await DbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetAll), null);
    }

    [HttpPut]
    public async Task<ActionResult> Update(
        string id,
        [FromBody] TUpdateRequest request,
        [FromServices] IValidator<TUpdateRequest> validator,
        [FromServices] UserContext userContext,
        CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                detail: "Unauthorized");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        TEntity? entity = await DbContext
            .Set<TEntity>()
            .FirstOrDefaultAsync(en => en.Id == id, cancellationToken);
        if (entity is null)
        {
            return Problem(
                detail: $"{nameof(TEntity)} with specific id does not exists!",
                statusCode: StatusCodes.Status404NotFound);
        }

        bool codeExists = await DbContext
            .Set<TEntity>()
            .AnyAsync(en => en.Code == request.Code && en.Id != entity.Id, cancellationToken);
        if (codeExists)
        {
            return Problem(
                detail: $"{nameof(TEntity)} with specific name already exists!",
                statusCode: StatusCodes.Status409Conflict);
        }

        entity.UpdateFromRequest(request, user.Email);
        await DbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
