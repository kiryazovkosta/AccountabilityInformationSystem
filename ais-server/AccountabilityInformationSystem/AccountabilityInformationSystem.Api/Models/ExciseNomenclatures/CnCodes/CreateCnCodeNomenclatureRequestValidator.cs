using AccountabilityInformationSystem.Api.Common.Constants;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AccountabilityInformationSystem.Api.Models.ExciseNomenclatures.CnCodes;

public sealed class CreateCnCodeNomenclatureRequestValidator : AbstractValidator<CreateCnCodeNomenclatureRequest>
{
    public CreateCnCodeNomenclatureRequestValidator()
    {
        Include(new CreateExciseNomenclatureValidator<CreateCnCodeNomenclatureRequest>(
            EntitiesConstants.CnCodeConstant.CodePattern,
            EntitiesConstants.CnCodeConstant.DescriptionMaxlength));
    }
}
