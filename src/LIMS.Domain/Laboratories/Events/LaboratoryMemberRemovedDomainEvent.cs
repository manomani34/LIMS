using LIMS.Domain.Common;

namespace LIMS.Domain.Laboratories.Events;

public sealed record LaboratoryMemberRemovedDomainEvent(
    Guid LaboratoryId,
    string UserId,
    DateTimeOffset OccurredOn) : IDomainEvent;
