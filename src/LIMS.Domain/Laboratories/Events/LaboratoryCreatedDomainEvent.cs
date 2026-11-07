using LIMS.Domain.Common;

namespace LIMS.Domain.Laboratories.Events;

public sealed record LaboratoryCreatedDomainEvent(
    Guid LaboratoryId,
    string Name,
    DateTimeOffset OccurredOn) : IDomainEvent;
