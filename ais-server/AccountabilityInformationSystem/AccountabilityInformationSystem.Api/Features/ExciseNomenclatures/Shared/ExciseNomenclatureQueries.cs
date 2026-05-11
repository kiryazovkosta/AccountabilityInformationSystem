using System.Linq.Expressions;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared;

internal sealed class ExciseNomenclatureQueries
{
    public static Expression<Func<TSource, ExciseNomenclatureResponse>> ProjectToResponse<TSource>()
        where TSource : class, IEntity, IExciseEntity
    {
        return exciseNomenclature => new ExciseNomenclatureResponse()
        {
            Id = exciseNomenclature.Id,
            Code = exciseNomenclature.Code,
            DescriptionBg = exciseNomenclature.DescriptionBg,
            DescriptionEn = exciseNomenclature.DescriptionEn,
            IsUsed = exciseNomenclature.IsUsed
        };
    }
}
