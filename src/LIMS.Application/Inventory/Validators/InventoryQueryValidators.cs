using LIMS.Application.Inventory.Queries;
using LIMS.Application.Abstractions.Validation;

namespace LIMS.Application.Inventory.Validators;

public sealed class GetInventoryItemByIdQueryValidator
    : AbstractValidator<GetInventoryItemByIdQuery>
{
    public GetInventoryItemByIdQueryValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Inventory item ID is required.");
    }
}

public sealed class GetInventoryItemByCodeQueryValidator
    : AbstractValidator<GetInventoryItemByCodeQuery>
{
    public GetInventoryItemByCodeQueryValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Laboratory ID is required.");

        RuleFor(x => x.ItemCode)
            .NotEmpty()
            .MaximumLength(50);
    }
}

public sealed class GetInventoryItemsByLaboratoryQueryValidator
    : AbstractValidator<GetInventoryItemsByLaboratoryQuery>
{
    public GetInventoryItemsByLaboratoryQueryValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Laboratory ID is required.");

        RuleFor(x => x.PageNumber)
            .Must(x => x >= 1)
            .WithMessage("Page number must be greater than zero.");

        RuleFor(x => x.PageSize)
            .Must(x => x is >= 1 and <= 200)
            .WithMessage("Page size must be between 1 and 200.");

        RuleFor(x => x.Status)
            .Must(x => !x.HasValue || Enum.IsDefined(x.Value))
            .WithMessage("Inventory status is invalid.");

        RuleFor(x => x.Type)
            .Must(x => !x.HasValue || Enum.IsDefined(x.Value))
            .WithMessage("Inventory item type is invalid.");
    }
}
