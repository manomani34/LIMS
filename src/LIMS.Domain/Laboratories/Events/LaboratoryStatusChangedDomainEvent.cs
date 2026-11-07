using LIMS.Domain.Common;
using LIMS.Domain.Laboratories.Enums;

namespace LIMS.Domain.Laboratories.Events;

public sealed record LaboratoryStatusChangedDomainEvent(
    Guid LaboratoryId,
    LaboratoryStatus PreviousStatus,
    LaboratoryStatus NewStatus,
    DateTimeOffset OccurredOn) : IDomainEvent;
