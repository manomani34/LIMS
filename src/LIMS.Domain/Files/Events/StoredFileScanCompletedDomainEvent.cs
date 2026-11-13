using LIMS.Domain.Common;
using LIMS.Domain.Files.Enums;

namespace LIMS.Domain.Files.Events;

public sealed record StoredFileScanCompletedDomainEvent(
    Guid FileId,
    FileScanStatus ScanStatus,
    DateTimeOffset OccurredOn) : IDomainEvent;

