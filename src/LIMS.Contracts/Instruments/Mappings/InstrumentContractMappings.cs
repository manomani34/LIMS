using LIMS.Domain.Instruments.Enums;

namespace LIMS.Contracts.Instruments.Mappings;

public static class InstrumentContractMappings
{
    public static InstrumentStatus ToDomain(this InstrumentStatusContract status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status), status, "Invalid instrument status.");
        }

        return (InstrumentStatus)(int)status;
    }

    public static InstrumentStatusContract ToContract(this InstrumentStatus status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status), status, "Invalid instrument status.");
        }

        return (InstrumentStatusContract)(int)status;
    }

    public static InstrumentType ToDomain(this InstrumentTypeContract type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid instrument type.");
        }

        return (InstrumentType)(int)type;
    }

    public static InstrumentTypeContract ToContract(this InstrumentType type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid instrument type.");
        }

        return (InstrumentTypeContract)(int)type;
    }

    public static MaintenanceType ToDomain(this MaintenanceTypeContract type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid maintenance type.");
        }

        return (MaintenanceType)(int)type;
    }

    public static MaintenanceTypeContract ToContract(this MaintenanceType type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid maintenance type.");
        }

        return (MaintenanceTypeContract)(int)type;
    }
}
