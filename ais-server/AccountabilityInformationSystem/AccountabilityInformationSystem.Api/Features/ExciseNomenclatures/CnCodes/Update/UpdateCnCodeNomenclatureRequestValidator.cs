using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Update;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.CnCodes.Update;

public sealed class UpdateCnCodeNomenclatureRequestValidator : AbstractValidator<UpdateCnCodeNomenclatureRequest>
{
    public UpdateCnCodeNomenclatureRequestValidator()
    {
        Include(new UpdateExciseNomenclatureValidator<UpdateCnCodeNomenclatureRequest>(
            EntitiesConstants.CnCodeConstants.CodeLength,
            EntitiesConstants.CnCodeConstants.CodePattern,
            EntitiesConstants.CnCodeConstants.DescriptionMaxlength));
    }
}
