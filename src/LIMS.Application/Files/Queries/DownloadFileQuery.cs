using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Files.DTOs;

namespace LIMS.Application.Files.Queries;

public sealed record DownloadFileQuery(Guid FileId)
    : IQuery<FileDownloadDto?>;
