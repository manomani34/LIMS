using LIMS.Domain.Common;

namespace LIMS.Domain.Protocols.Events;

public sealed record ProtocolUpdatedDomainEvent(
    Guid ProtocolId,
    DateTimeOffset OccurredOn) : IDomainEvent;
