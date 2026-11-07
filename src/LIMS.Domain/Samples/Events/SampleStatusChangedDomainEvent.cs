using LIMS.Domain.Common;
using LIMS.Domain.Samples.Enums;

namespace LIMS.Domain.Samples.Events;

public sealed record SampleStatusChangedDomainEvent(
    Guid SampleId,
    SampleStatus PreviousStatus,
    SampleStatus NewStatus,
    DateTimeOffset OccurredOn) : IDomainEvent;
