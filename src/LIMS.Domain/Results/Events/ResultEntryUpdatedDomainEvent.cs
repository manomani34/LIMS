using LIMS.Domain.Common;

namespace LIMS.Domain.Results.Events;

public sealed record ResultEntryUpdatedDomainEvent(
    Guid ResultId,
    Guid EntryId,
    DateTimeOffset OccurredOn) : IDomainEvent;
