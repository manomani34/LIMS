namespace LIMS.Contracts.Inventory.Enums;

public enum InventoryMovementTypeContract
{
    Receipt = 1,
    Consumption = 2,
    AdjustmentIncrease = 3,
    AdjustmentDecrease = 4,
    TransferIn = 5,
    TransferOut = 6,
    Disposal = 7,
    Return = 8
}
