using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Instruments.Commands;

public sealed record CompleteInstrumentMaintenanceCommand(
    Guid InstrumentId,
    Guid MaintenanceId,
    string CompletedByUserId,
    string? CompletionNotes) : ICommand<bool>;
