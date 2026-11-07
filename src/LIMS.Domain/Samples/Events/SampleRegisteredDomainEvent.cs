using LIMS.Domain.Common;

namespace LIMS.Domain.Samples.Events;

public sealed record SampleRegisteredDomainEvent(
    Guid SampleId,
    Guid LaboratoryId,
    string SampleCode,
    DateTimeOffset OccurredOn) : IDomainEvent;
