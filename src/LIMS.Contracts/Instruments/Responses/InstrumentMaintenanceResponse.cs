namespace LIMS.Contracts.Instruments.Responses;

public sealed record InstrumentMaintenanceResponse(
    Guid Id,
    Guid InstrumentId,
    MaintenanceTypeContract Type,
    string Description,
    DateTimeOffset ScheduledAt,
    string ScheduledByUserId,
    DateTimeOffset? CompletedAt,
    string? CompletedByUserId,
    string? CompletionNotes);
