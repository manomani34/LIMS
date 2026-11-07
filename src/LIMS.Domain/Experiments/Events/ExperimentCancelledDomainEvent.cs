using LIMS.Domain.Common;

namespace LIMS.Domain.Experiments.Events;

public sealed record ExperimentCancelledDomainEvent(
    Guid ExperimentId,
    Guid SampleId,
    string Reason,
    DateTimeOffset OccurredOn) : IDomainEvent;
