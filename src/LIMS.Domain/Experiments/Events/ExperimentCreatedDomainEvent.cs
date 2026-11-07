using LIMS.Domain.Common;

namespace LIMS.Domain.Experiments.Events;

public sealed record ExperimentCreatedDomainEvent(
    Guid ExperimentId,
    Guid LaboratoryId,
    Guid SampleId,
    string ExperimentCode,
    DateTimeOffset OccurredOn) : IDomainEvent;
