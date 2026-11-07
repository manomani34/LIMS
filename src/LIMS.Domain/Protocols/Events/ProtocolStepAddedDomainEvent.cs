using LIMS.Domain.Common;

namespace LIMS.Domain.Protocols.Events;

public sealed record ProtocolStepAddedDomainEvent(
    Guid ProtocolId,
    Guid StepId,
    int Sequence,
    DateTimeOffset OccurredOn) : IDomainEvent;
