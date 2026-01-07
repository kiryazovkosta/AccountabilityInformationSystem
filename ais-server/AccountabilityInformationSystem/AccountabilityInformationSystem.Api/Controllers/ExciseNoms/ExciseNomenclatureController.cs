using System.Dynamic;
using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Entities.Excise;
using AccountabilityInformationSystem.Api.Entities.Identity;
using AccountabilityInformationSystem.Api.Extensions;
using AccountabilityInformationSystem.Api.Models.Common;
using AccountabilityInformationSystem.Api.Models.ExciseNomenclatures;
using AccountabilityInformationSystem.Api.Services.DataShaping;
using AccountabilityInformationSystem.Api.Services.Sorting;
using AccountabilityInformationSystem.Api.Services.UserContexting;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Controllers.ExciseNoms;

[Authorize]
[ApiController]
public abstract class ExciseNomenclatureController<TEntity, TCreateRequest> : ControllerBase
    where TEntity : class, IEntity, IExciseEntity
    where TCreateRequest: CreateExciseNomenclatureRequest
{
    protected readonly ApplicationDbContext DbContext;

    protected ExciseNomenclatureController(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        DbContext = dbContext;
    }

    [HttpGet]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> GetCodes(
        [FromQuery] ExciseNomenclatureQueryParameters query,
        SortMappingProvider sortMappingProvider,
        DataShapingService dataShapingService,
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
    public async Task<IActionResult> GetCode(
        string id,
        [FromQuery] FieldsOnlyQueryParameters query,
        DataShapingService dataShapingService,
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
        IValidator<TCreateRequest> validator,
        UserContext userContext,
        CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Unauthorized");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        if (await DbContext.Set<ApCode>().AnyAsync(en => en.Code == request.Code, cancellationToken))
        {
            return Problem(
                detail: $"{typeof(ApCode)} with specific code already exists!",
                statusCode: StatusCodes.Status409Conflict);
        }

        ApCode entity = request.ToEntity<ApCode>(user.Email, "ac");
        await DbContext.Set<ApCode>().AddAsync(entity, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
        ExciseNomenclatureResponse response = entity.ToResponse();

        return CreatedAtAction(
            actionName: nameof(GetCode),
            routeValues: new { id = response.Id },
            value: response);
    }
}
