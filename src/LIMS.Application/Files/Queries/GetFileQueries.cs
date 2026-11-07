using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Files.DTOs;
using LIMS.Domain.Files.Enums;

namespace LIMS.Application.Files.Queries;

public sealed record GetFileByIdQuery(
    Guid FileId) : IQuery<FileAssetDto?>;

public sealed record GetFilesByReferenceQuery(
    FileReferenceType ReferenceType,
    Guid ReferenceId) : IQuery<IReadOnlyCollection<FileAssetDto>>;

public sealed record GetFileReferencesQuery(
    Guid FileId) : IQuery<IReadOnlyCollection<FileReferenceDto>>;
