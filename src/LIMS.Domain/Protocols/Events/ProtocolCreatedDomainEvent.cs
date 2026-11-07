using LIMS.Domain.Common;

namespace LIMS.Domain.Protocols.Events;

public sealed record ProtocolCreatedDomainEvent(
    Guid ProtocolId,
    Guid LaboratoryId,
    string ProtocolCode,
    DateTimeOffset OccurredOn) : IDomainEvent;
