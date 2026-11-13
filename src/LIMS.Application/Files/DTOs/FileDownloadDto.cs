namespace LIMS.Application.Files.DTOs;

public sealed record FileDownloadDto(
    Stream Content,
    string FileName,
    string ContentType,
    long SizeInBytes);
