using AccountabilityInformationSystem.Api.Common.Constants;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AccountabilityInformationSystem.Api.Models.ExciseNomenclatures.BrandNames;

public sealed class CreateBrandNameNomenclatureRequestValidator : AbstractValidator<CreateBrandNameNomenclatureRequest>
{
    public CreateBrandNameNomenclatureRequestValidator()
    {
        Include(new CreateExciseNomenclatureValidator<CreateBrandNameNomenclatureRequest>(
            EntitiesConstants.BrandNameConstants.CodePattern,
            EntitiesConstants.BrandNameConstants.DescriptionMaxlength));
    }
}
