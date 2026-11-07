using LIMS.Domain.Common;
using LIMS.Domain.Instruments.Enums;

namespace LIMS.Domain.Instruments.Events;

public sealed record InstrumentStatusChangedDomainEvent(
    Guid InstrumentId,
    InstrumentStatus PreviousStatus,
    InstrumentStatus NewStatus,
    DateTimeOffset OccurredOn) : IDomainEvent;
