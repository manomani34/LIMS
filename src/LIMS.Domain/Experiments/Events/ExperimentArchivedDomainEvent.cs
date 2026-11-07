using LIMS.Domain.Common;

namespace LIMS.Domain.Experiments.Events;

public sealed record ExperimentArchivedDomainEvent(
    Guid ExperimentId,
    DateTimeOffset OccurredOn) : IDomainEvent;
