using LIMS.Domain.Common;

namespace LIMS.Domain.Samples.Events;

public sealed record SampleAssignedDomainEvent(
    Guid SampleId,
    string UserId,
    DateTimeOffset OccurredOn) : IDomainEvent;
