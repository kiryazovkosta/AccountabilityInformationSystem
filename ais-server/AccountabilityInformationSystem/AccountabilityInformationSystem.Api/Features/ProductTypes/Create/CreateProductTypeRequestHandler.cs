using AccountabilityInformationSystem.Api.Domain.Entities;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.ProductTypes.Create;
using AccountabilityInformationSystem.Api.Features.ProductTypes.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.ProductTypes.Create;

public sealed class CreateProductTypeRequestHandler(
    ApplicationDbContext dbContext,
    UserContext userContext)
{
    public async Task<Result<string>> Handle(CreateProductTypeRequest request, CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Result< string>.Failure(new Error("user", "Unauthorized"), ResultFailureType.Unauthorized);
        }

        if (await dbContext.ProductTypes.AnyAsync(mp => mp.Name == request.Name, cancellationToken))
        {
            return Result<string>.Failure(
                new Error("name", "Product type with specific name already exists!"),
                ResultFailureType.Conflict);
        }

        ProductType productType = request.Adapt<ProductType>();
        await dbContext.ProductTypes.AddAsync(productType, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        ProductTypeResponse productTypeResponse = productType.Adapt<ProductTypeResponse>();
        return Result<string>.Success(productTypeResponse.Id);
    } 
}
