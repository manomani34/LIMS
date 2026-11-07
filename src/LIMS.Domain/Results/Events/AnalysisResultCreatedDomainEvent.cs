using LIMS.Domain.Common;

namespace LIMS.Domain.Results.Events;

public sealed record AnalysisResultCreatedDomainEvent(
    Guid ResultId,
    Guid LaboratoryId,
    Guid ExperimentId,
    Guid SampleId,
    string ResultCode,
    DateTimeOffset OccurredOn) : IDomainEvent;
