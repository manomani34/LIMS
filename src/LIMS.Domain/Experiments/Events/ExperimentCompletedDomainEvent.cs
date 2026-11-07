using LIMS.Domain.Common;

namespace LIMS.Domain.Experiments.Events;

public sealed record ExperimentCompletedDomainEvent(
    Guid ExperimentId,
    Guid SampleId,
    DateTimeOffset CompletedAt,
    DateTimeOffset OccurredOn) : IDomainEvent;
