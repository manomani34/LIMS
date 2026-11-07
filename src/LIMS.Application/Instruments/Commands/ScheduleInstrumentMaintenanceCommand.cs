using LIMS.Application.Abstractions.Messaging;
using LIMS.Domain.Instruments.Enums;

namespace LIMS.Application.Instruments.Commands;

public sealed record ScheduleInstrumentMaintenanceCommand(
    Guid InstrumentId,
    MaintenanceType Type,
    string Description,
    DateTimeOffset ScheduledAt,
    string ScheduledByUserId) : ICommand<Guid>;
