using LIMS.Domain.Common;
using LIMS.Domain.Files.Enums;

namespace LIMS.Domain.Files.Events;

public sealed record StoredFileReferenceAddedDomainEvent(
    Guid FileId,
    FileReferenceType ReferenceType,
    Guid ReferenceId,
    DateTimeOffset OccurredOn) : IDomainEvent;

