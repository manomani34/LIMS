using LIMS.Domain.Common;

namespace LIMS.Domain.Instruments.Events;

public sealed record InstrumentRegisteredDomainEvent(
    Guid InstrumentId,
    Guid LaboratoryId,
    string InstrumentCode,
    DateTimeOffset OccurredOn) : IDomainEvent;
