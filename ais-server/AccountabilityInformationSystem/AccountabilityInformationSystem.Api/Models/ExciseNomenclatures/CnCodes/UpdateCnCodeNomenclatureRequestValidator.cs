using AccountabilityInformationSystem.Api.Common.Constants;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Models.ExciseNomenclatures.CnCodes;

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
