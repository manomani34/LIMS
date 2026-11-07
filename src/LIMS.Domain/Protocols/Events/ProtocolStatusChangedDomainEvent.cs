using LIMS.Domain.Common;
using LIMS.Domain.Protocols.Enums;

namespace LIMS.Domain.Protocols.Events;

public sealed record ProtocolStatusChangedDomainEvent(
    Guid ProtocolId,
    ProtocolStatus PreviousStatus,
    ProtocolStatus NewStatus,
    DateTimeOffset OccurredOn) : IDomainEvent;
