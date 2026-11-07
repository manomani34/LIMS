using LIMS.Domain.Common;

namespace LIMS.Domain.Instruments.Events;

public sealed record InstrumentMaintenanceCompletedDomainEvent(
    Guid InstrumentId,
    Guid MaintenanceId,
    DateTimeOffset CompletedAt,
    DateTimeOffset OccurredOn) : IDomainEvent;
