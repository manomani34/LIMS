using LIMS.Domain.Common;

namespace LIMS.Domain.Requests.Events;

public sealed record SampleRequestApprovedDomainEvent(
    Guid RequestId,
    string ApprovedByUserId,
    Guid LaboratoryId,
    DateTime ApprovedAt,
    DateTimeOffset OccurredOn) : IDomainEvent;

