using LIMS.Domain.Common;
using LIMS.Domain.Files.Enums;

namespace LIMS.Domain.Files.Events;

public sealed record StoredFileReferenceRemovedDomainEvent(
    Guid FileId,
    FileReferenceType ReferenceType,
    Guid ReferenceId,
    DateTime OccurredOn) : IDomainEvent;
