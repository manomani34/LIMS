using LIMS.Domain.Protocols.Enums;

namespace LIMS.Contracts.Protocols;

public static class ProtocolStatusContractExtensions
{
    public static ProtocolStatus ToDomain(this ProtocolStatusContract status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(
                nameof(status),
                status,
                "The protocol status is not supported.");
        }

        return (ProtocolStatus)(int)status;
    }

    public static ProtocolStatusContract ToContract(this ProtocolStatus status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(
                nameof(status),
                status,
                "The protocol status is not supported.");
        }

        return (ProtocolStatusContract)(int)status;
    }
}
