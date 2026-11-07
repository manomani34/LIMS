using LIMS.Domain.Common;
using LIMS.Domain.Instruments.Enums;

namespace LIMS.Domain.Instruments.Events;

public sealed record InstrumentMaintenanceScheduledDomainEvent(
    Guid InstrumentId,
    Guid MaintenanceId,
    MaintenanceType Type,
    DateTimeOffset ScheduledAt,
    DateTimeOffset OccurredOn) : IDomainEvent;
