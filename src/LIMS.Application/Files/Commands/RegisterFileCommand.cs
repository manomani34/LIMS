using LIMS.Application.Abstractions.Messaging;
using LIMS.Domain.Files.Enums;

namespace LIMS.Application.Files.Commands;

public sealed record RegisterFileCommand(
    string OriginalFileName,
    string StorageKey,
    string ContentType,
    string FileExtension,
    long SizeInBytes,
    string Sha256Checksum,
    FileCategory Category,
    string UploadedByUserId,
    string? Description) : ICommand<Guid>;
