using System.Linq.Expressions;
using AccountabilityInformationSystem.Api.Entities;

namespace AccountabilityInformationSystem.Api.Models.ProductTypes;

public static class ProductTypeQueries
{
    public static Expression<Func<ProductType, ProductTypeResponse>> ProjectToResponse()
    {
        return mp => new ProductTypeResponse
        {
            Id = mp.Id,
            Name = mp.Name,
            FullName = mp.FullName,
            Products = mp.Products.Select(p => new ProductTypeProductResponse()
            {
                Id = p.Id,
                Code = p.Code,
                FullName = p.FullName
            }).ToList()
        };
    }
}
