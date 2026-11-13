using LIMS.Application.Abstractions.Validation;
using LIMS.Application.Inventory.Commands;

namespace LIMS.Application.Inventory.Validators;

public sealed class CreateInventoryItemCommandValidator
    : AbstractValidator<CreateInventoryItemCommand>
{
    public CreateInventoryItemCommandValidator()
    {
        RuleFor(x => x.LaboratoryId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Laboratory ID is required.");

        RuleFor(x => x.ItemCode)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[A-Za-z0-9_-]+$")
            .WithMessage("Item code contains invalid characters.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.Type)
            .Must(x => Enum.IsDefined(x))
            .WithMessage("Inventory item type is invalid.");

        RuleFor(x => x.CasNumber)
            .MaximumLength(50);

        RuleFor(x => x.ChemicalFormula)
            .MaximumLength(100);

        RuleFor(x => x.MolecularWeight)
            .MaximumLength(50);

        RuleFor(x => x.HazardClassification)
            .MaximumLength(1000);

        RuleFor(x => x.SafetyDataSheetReference)
            .MaximumLength(500);

        RuleFor(x => x.DefaultUnit)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.MinimumStockLevel)
            .Must(x => x >= 0)
            .WithMessage("Minimum stock level cannot be negative.");
    }
}

public sealed class UpdateInventoryItemCommandValidator
    : AbstractValidator<UpdateInventoryItemCommand>
{
    public UpdateInventoryItemCommandValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Inventory item ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.Type)
            .Must(x => Enum.IsDefined(x))
            .WithMessage("Inventory item type is invalid.");

        RuleFor(x => x.CasNumber).MaximumLength(50);
        RuleFor(x => x.ChemicalFormula).MaximumLength(100);
        RuleFor(x => x.MolecularWeight).MaximumLength(50);
        RuleFor(x => x.HazardClassification).MaximumLength(1000);
        RuleFor(x => x.SafetyDataSheetReference).MaximumLength(500);

        RuleFor(x => x.DefaultUnit)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.MinimumStockLevel)
            .Must(x => x >= 0)
            .WithMessage("Minimum stock level cannot be negative.");
    }
}

public sealed class ReceiveInventoryStockCommandValidator
    : AbstractValidator<ReceiveInventoryStockCommand>
{
    public ReceiveInventoryStockCommandValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Inventory item ID is required.");

        RuleFor(x => x.LotNumber)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Supplier).MaximumLength(200);
        RuleFor(x => x.Manufacturer).MaximumLength(200);

        RuleFor(x => x.Quantity)
            .Must(x => x > 0)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.Unit)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.MinimumStockLevel)
            .Must(x => !x.HasValue || x.Value >= 0)
            .WithMessage("Minimum stock level cannot be negative.");

        RuleFor(x => x.StorageLocation).MaximumLength(200);
        RuleFor(x => x.StorageConditions).MaximumLength(1000);
        RuleFor(x => x.CertificateOfAnalysisNumber).MaximumLength(150);

        RuleFor(x => x.PerformedByUserId)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.ReferenceNumber).MaximumLength(150);
        RuleFor(x => x.Notes).MaximumLength(2000);

        RuleFor(x => x.ExpiryDate)
            .Must((command, expiryDate) =>
                !expiryDate.HasValue
                || !command.ManufactureDate.HasValue
                || expiryDate.Value >= command.ManufactureDate.Value)
            .WithMessage(
                "Expiry date cannot be earlier than manufacture date.");
    }
}

public sealed class ConsumeInventoryStockCommandValidator
    : AbstractValidator<ConsumeInventoryStockCommand>
{
    public ConsumeInventoryStockCommandValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Inventory item ID is required.");

        RuleFor(x => x.LotId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Lot ID is required.");

        RuleFor(x => x.Quantity)
            .Must(x => x > 0)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.ConsumedByUserId)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.ExperimentId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("Experiment ID cannot be empty.");

        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}

public sealed class AdjustInventoryStockCommandValidator
    : AbstractValidator<AdjustInventoryStockCommand>
{
    public AdjustInventoryStockCommandValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Inventory item ID is required.");

        RuleFor(x => x.LotId)
            .Must(x => x != Guid.Empty)
            .WithMessage("Lot ID is required.");

        RuleFor(x => x.Quantity)
            .Must(x => x > 0)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.PerformedByUserId)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
