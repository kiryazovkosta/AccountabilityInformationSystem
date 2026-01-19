using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.CreateExciseNomenclature;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.CnCodes.CreateCnCode;

public sealed class CreateCnCodeNomenclatureRequestValidator : AbstractValidator<CreateCnCodeNomenclatureRequest>
{
    public CreateCnCodeNomenclatureRequestValidator()
    {
        Include(new CreateExciseNomenclatureValidator<CreateCnCodeNomenclatureRequest>(
            EntitiesConstants.CnCodeConstants.CodePattern,
            EntitiesConstants.CnCodeConstants.DescriptionMaxlength));
    }
}
