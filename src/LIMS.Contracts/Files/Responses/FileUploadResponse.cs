namespace LIMS.Contracts.Files.Responses;

public sealed record FileUploadResponse(
    Guid Id,
    string OriginalFileName,
    long SizeInBytes,
    string Sha256Checksum,
    string Status);
