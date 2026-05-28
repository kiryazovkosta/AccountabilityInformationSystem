using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Create;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.CnCodes.Create;

public sealed class CreateCnCodeNomenclatureRequestValidator : AbstractValidator<CreateCnCodeNomenclatureRequest>
{
    public CreateCnCodeNomenclatureRequestValidator()
    {
        Include(new CreateExciseNomenclatureValidator<CreateCnCodeNomenclatureRequest>(
            EntitiesConstants.CnCodeConstants.CodePattern,
            EntitiesConstants.CnCodeConstants.DescriptionMaxlength));
    }
}
