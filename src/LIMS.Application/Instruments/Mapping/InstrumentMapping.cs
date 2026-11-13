
using LIMS.Application.Instruments.DTOs;
using LIMS.Domain.Instruments;

namespace LIMS.Application.Instruments.Mapping;

public static class InstrumentMapping
{
    public static InstrumentDto ToDto(this Instrument instrument)
    {
        ArgumentNullException.ThrowIfNull(instrument);

        return new InstrumentDto(
            instrument.Id,
            instrument.LaboratoryId,
            instrument.InstrumentCode,
            instrument.Name,
            instrument.Type,
            instrument.Status,
            instrument.Manufacturer,
            instrument.Model,
            instrument.SerialNumber,
            instrument.Location,
            instrument.Description,
            instrument.RegisteredAt,
            instrument.UpdatedAt,
            instrument.RetiredAt);
    }

    public static InstrumentDetailsDto ToDetailsDto(
        this Instrument instrument)
    {
        ArgumentNullException.ThrowIfNull(instrument);

        return new InstrumentDetailsDto(
            instrument.Id,
            instrument.LaboratoryId,
            instrument.InstrumentCode,
            instrument.Name,
            instrument.Type,
            instrument.Status,
            instrument.Manufacturer,
            instrument.Model,
            instrument.SerialNumber,
            instrument.Location,
            instrument.Description,
            instrument.RegisteredAt,
            instrument.UpdatedAt,
            instrument.RetiredAt,
            instrument.MaintenanceRecords
                .Select(x => x.ToDto())
                .ToArray());
    }

    public static InstrumentMaintenanceDto ToDto(
        this InstrumentMaintenance maintenance)
    {
        ArgumentNullException.ThrowIfNull(maintenance);

        return new InstrumentMaintenanceDto(
            maintenance.Id,
            maintenance.InstrumentId,
            maintenance.Type,
            maintenance.Description,
            maintenance.ScheduledAt,
            maintenance.ScheduledByUserId,
            maintenance.CompletedAt,
            maintenance.CompletedByUserId,
            maintenance.CompletionNotes,
            maintenance.IsCompleted);
    }
}
