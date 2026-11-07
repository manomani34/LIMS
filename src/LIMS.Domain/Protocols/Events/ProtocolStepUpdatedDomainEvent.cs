using LIMS.Domain.Common;

namespace LIMS.Domain.Protocols.Events;

public sealed record ProtocolStepUpdatedDomainEvent(
    Guid ProtocolId,
    Guid StepId,
    DateTimeOffset OccurredOn) : IDomainEvent;
