using LIMS.Domain.Common;

namespace LIMS.Domain.Results.Events;

public sealed record ResultEntryRemovedDomainEvent(
    Guid ResultId,
    Guid EntryId,
    string ParameterName,
    DateTimeOffset OccurredOn) : IDomainEvent;
