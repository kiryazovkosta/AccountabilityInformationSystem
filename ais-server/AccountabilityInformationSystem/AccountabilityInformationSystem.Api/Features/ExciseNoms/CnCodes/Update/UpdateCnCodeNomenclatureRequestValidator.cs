using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Update;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.CnCodes.Update;

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
