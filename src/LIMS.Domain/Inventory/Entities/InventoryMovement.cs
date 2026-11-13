using LIMS.Domain.Common;
using LIMS.Domain.Inventory.Enums;
using LIMS.Domain.Inventory.Exceptions;

namespace LIMS.Domain.Inventory.Entities;

public sealed class InventoryMovement : Entity<Guid>
{
    public Guid LotId { get; private set; }

    public InventoryMovementType Type { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal BalanceAfterMovement { get; private set; }

    public string Unit { get; private set; } = string.Empty;

    public string PerformedByUserId { get; private set; } = string.Empty;

    public Guid? ExperimentId { get; private set; }

    public string? ReferenceNumber { get; private set; }

    public string? Notes { get; private set; }

    public DateTime OccurredAt { get; private set; }

    private InventoryMovement()
    {
    }

    internal static InventoryMovement Create(
        Guid lotId,
        InventoryMovementType type,
        decimal quantity,
        decimal balanceAfterMovement,
        string unit,
        string performedByUserId,
        Guid? experimentId,
        string? referenceNumber,
        string? notes,
        DateTime occurredAt)
    {
        if (lotId == Guid.Empty)
            throw new InventoryDomainException(
                "Lot identifier is required.");

        if (!Enum.IsDefined(type))
            throw new InventoryDomainException(
                "Invalid inventory movement type.");

        if (quantity <= 0)
            throw new InventoryDomainException(
                "Movement quantity must be greater than zero.");

        if (balanceAfterMovement < 0)
            throw new InventoryDomainException(
                "Movement balance cannot be negative.");

        if (string.IsNullOrWhiteSpace(unit))
            throw new InventoryDomainException(
                "Movement unit is required.");

        if (unit.Trim().Length > 30)
            throw new InventoryDomainException(
                "Movement unit cannot exceed 30 characters.");

        if (string.IsNullOrWhiteSpace(performedByUserId))
            throw new InventoryDomainException(
                "Performing user identifier is required.");

        if (performedByUserId.Trim().Length > 150)
            throw new InventoryDomainException(
                "Performing user identifier cannot exceed 150 characters.");

        if (experimentId == Guid.Empty)
            throw new InventoryDomainException(
                "Experiment identifier cannot be empty.");

        ValidateOptionalLength(referenceNumber, 150, nameof(referenceNumber));
        ValidateOptionalLength(notes, 2000, nameof(notes));

        return new InventoryMovement
        {
            Id = Guid.NewGuid(),
            LotId = lotId,
            Type = type,
            Quantity = quantity,
            BalanceAfterMovement = balanceAfterMovement,
            Unit = unit.Trim(),
            PerformedByUserId = performedByUserId.Trim(),
            ExperimentId = experimentId,
            ReferenceNumber = NormalizeOptional(referenceNumber),
            Notes = NormalizeOptional(notes),
            OccurredAt = occurredAt
        };
    }

    private static void ValidateOptionalLength(
        string? value,
        int maximumLength,
        string propertyName)
    {
        if (value?.Trim().Length > maximumLength)
        {
            throw new InventoryDomainException(
                $"{propertyName} cannot exceed {maximumLength} characters.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
