using LIMS.Domain.Common;

namespace LIMS.Domain.Files.Events;

public sealed record StoredFileRegisteredDomainEvent(
    Guid FileId,
    string OriginalFileName,
    DateTimeOffset OccurredOn) : IDomainEvent;

