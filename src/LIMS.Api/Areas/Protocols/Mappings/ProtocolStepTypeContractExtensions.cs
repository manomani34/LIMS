using LIMS.Domain.Protocols.Enums;

namespace LIMS.Contracts.Protocols;

public static class ProtocolStepTypeContractExtensions
{
    public static ProtocolStepType ToDomain(this ProtocolStepTypeContract type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "The protocol step type is not supported.");
        }

        return (ProtocolStepType)(int)type;
    }

    public static ProtocolStepTypeContract ToContract(this ProtocolStepType type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "The protocol step type is not supported.");
        }

        return (ProtocolStepTypeContract)(int)type;
    }
}
