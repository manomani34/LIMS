using LIMS.Domain.Common;

namespace LIMS.Domain.Results.Events;

public sealed record ResultEntryAddedDomainEvent(
    Guid ResultId,
    Guid EntryId,
    string ParameterName,
    DateTimeOffset OccurredOn) : IDomainEvent;
