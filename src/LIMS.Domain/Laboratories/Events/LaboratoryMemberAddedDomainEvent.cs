using LIMS.Domain.Common;
using LIMS.Domain.Laboratories.Enums;

namespace LIMS.Domain.Laboratories.Events;

public sealed record LaboratoryMemberAddedDomainEvent(
    Guid LaboratoryId,
    string UserId,
    LaboratoryMemberRole Role,
    DateTimeOffset OccurredOn) : IDomainEvent;
