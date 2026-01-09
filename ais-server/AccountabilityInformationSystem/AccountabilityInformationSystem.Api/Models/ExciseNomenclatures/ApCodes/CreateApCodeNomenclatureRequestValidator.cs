using AccountabilityInformationSystem.Api.Common.Constants;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AccountabilityInformationSystem.Api.Models.ExciseNomenclatures.ApCodes;

public sealed class CreateApCodeNomenclatureRequestValidator : AbstractValidator<CreateApCodeNomenclatureRequest>
{
    public CreateApCodeNomenclatureRequestValidator()
    {
        Include(new CreateExciseNomenclatureValidator<CreateApCodeNomenclatureRequest>(
            EntitiesConstants.ApCodeConstants.CodePattern,
            EntitiesConstants.ApCodeConstants.DescriptionMaxlength));
    }
}
