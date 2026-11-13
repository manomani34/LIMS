using LIMS.Domain.Common;

namespace LIMS.Domain.Files.Events;

public sealed record StoredFileDeletedDomainEvent(
    Guid FileId,
    DateTimeOffset OccurredOn) : IDomainEvent;

