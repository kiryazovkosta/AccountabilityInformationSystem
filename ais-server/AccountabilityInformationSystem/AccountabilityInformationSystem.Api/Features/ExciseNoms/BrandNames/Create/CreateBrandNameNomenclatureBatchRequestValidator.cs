using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Create;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.BrandNames.Create;

public sealed class CreateBrandNameNomenclatureBatchRequestValidator
    : AbstractValidator<CreateExciseNomenclatureBatchRequest<CreateBrandNameNomenclatureRequest>>
{
    public CreateBrandNameNomenclatureBatchRequestValidator(IValidator<CreateBrandNameNomenclatureRequest> entryValidator)
    {
        Include(new CreateExciseNomenclatureBatchRequestValidator<CreateBrandNameNomenclatureRequest>(entryValidator));
    }
}
