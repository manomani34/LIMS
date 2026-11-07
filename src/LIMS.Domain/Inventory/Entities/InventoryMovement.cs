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

        if (quantity <= 0)
            throw new InventoryDomainException(
                "Movement quantity must be greater than zero.");

        if (balanceAfterMovement < 0)
            throw new InventoryDomainException(
                "Movement balance cannot be negative.");

        if (string.IsNullOrWhiteSpace(unit))
            throw new InventoryDomainException(
                "Movement unit is required.");

        if (string.IsNullOrWhiteSpace(performedByUserId))
            throw new InventoryDomainException(
                "Performing user identifier is required.");

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

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
