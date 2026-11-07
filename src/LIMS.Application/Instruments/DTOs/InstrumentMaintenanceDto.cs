using LIMS.Domain.Instruments.Enums;

namespace LIMS.Application.Instruments.DTOs;

public sealed record InstrumentMaintenanceDto(
    Guid Id,
    Guid InstrumentId,
    MaintenanceType Type,
    string Description,
    DateTimeOffset ScheduledAt,
    string ScheduledByUserId,
    DateTimeOffset? CompletedAt,
    string? CompletedByUserId,
    string? CompletionNotes,
    bool IsCompleted);
