
using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Inventory.Commands;

namespace LIMS.Application.Inventory.Validators;

public sealed class DeactivateInventoryItemCommandValidator
    : AbstractValidator<DeactivateInventoryItemCommand>
{
    public DeactivateInventoryItemCommandValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Inventory item ID is required.");
    }
}

public sealed class ReactivateInventoryItemCommandValidator
    : AbstractValidator<ReactivateInventoryItemCommand>
{
    public ReactivateInventoryItemCommandValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Inventory item ID is required.");
    }
}

public sealed class DiscontinueInventoryItemCommandValidator
    : AbstractValidator<DiscontinueInventoryItemCommand>
{
    public DiscontinueInventoryItemCommandValidator()
    {
        RuleFor(x => x.InventoryItemId)
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
            .MaximumLength(150)
            .WithMessage("Performing user identifier is required and cannot exceed 150 characters.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(2000)
            .WithMessage("Disposal reason is required and cannot exceed 2000 characters.");
    }
}
