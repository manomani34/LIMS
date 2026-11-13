using LIMS.Contracts.Files;

namespace LIMS.Api.Areas.Files.Models;

public sealed class UploadFileRequest
{
    public IFormFile? File { get; init; }

    public FileCategoryContract Category { get; init; }

    public string UploadedByUserId { get; init; } = string.Empty;

    public string? Description { get; init; }
}
