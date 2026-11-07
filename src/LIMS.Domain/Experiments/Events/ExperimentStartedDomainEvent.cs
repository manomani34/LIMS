using LIMS.Domain.Common;

namespace LIMS.Domain.Experiments.Events;

public sealed record ExperimentStartedDomainEvent(
    Guid ExperimentId,
    Guid SampleId,
    DateTimeOffset StartedAt,
    DateTimeOffset OccurredOn) : IDomainEvent;
