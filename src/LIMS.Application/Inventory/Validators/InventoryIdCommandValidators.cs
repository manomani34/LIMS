using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Inventory.Commands;

namespace LIMS.Application.Inventory.Validators;

public sealed class InventoryItemIdCommandValidator<TCommand>
    : AbstractValidator<TCommand>
    where TCommand : class
{
    public InventoryItemIdCommandValidator(
        Func<TCommand, Guid> idSelector)
    {
        RuleForSelector(idSelector, "InventoryItemId")
            .Must(x => x != Guid.Empty)
            .WithMessage("Inventory item ID is required.");
    }
}

public sealed class QuarantineInventoryLotCommandValidator
    : AbstractValidator<QuarantineInventoryLotCommand>
{
    public QuarantineInventoryLotCommandValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Inventory item ID is required.");

        RuleFor(x => x.LotId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Lot ID is required.");
    }
}

public sealed class ReleaseInventoryLotCommandValidator
    : AbstractValidator<ReleaseInventoryLotCommand>
{
    public ReleaseInventoryLotCommandValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Inventory item ID is required.");

        RuleFor(x => x.LotId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Lot ID is required.");
    }
}

public sealed class DisposeInventoryLotCommandValidator
    : AbstractValidator<DisposeInventoryLotCommand>
{
    public DisposeInventoryLotCommandValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Inventory item ID is required.");

        RuleFor(x => x.LotId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Lot ID is required.");

        RuleFor(x => x.PerformedByUserId)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
