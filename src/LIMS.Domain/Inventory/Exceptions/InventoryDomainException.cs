using LIMS.Domain.Common;

namespace LIMS.Domain.Inventory.Exceptions;

public sealed class InventoryDomainException : DomainException
{
    public InventoryDomainException(string message)
        : base(message)
    {
    }
}
