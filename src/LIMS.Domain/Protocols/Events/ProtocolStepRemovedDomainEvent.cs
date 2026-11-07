using LIMS.Domain.Common;

namespace LIMS.Domain.Protocols.Events;

public sealed record ProtocolStepRemovedDomainEvent(
    Guid ProtocolId,
    Guid StepId,
    DateTimeOffset OccurredOn) : IDomainEvent;
