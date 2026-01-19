using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.UpdateExciseNomenclature;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.CnCodes.UpdateCnCode;

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
