using System.Security.Cryptography;
using LIMS.Api.Areas.Files.Mappings;
using LIMS.Api.Areas.Files.Models;
using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Storage;
using LIMS.Application.Files.Commands;
using LIMS.Application.Files.DTOs;
using LIMS.Application.Files.Queries;
using LIMS.Contracts.Common;
using LIMS.Contracts.Files;
using LIMS.Contracts.Files.Responses;
using LIMS.Domain.Files.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Api.Areas.Files.Controllers;

[Area("Files")]
[ApiController]
[Route("api/v1/files")]
[Produces("application/json")]
public sealed class FilesController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    private readonly IObjectStorage _objectStorage;

    public FilesController(
        IDispatcher dispatcher,
        IObjectStorage objectStorage)
    {
        _dispatcher = dispatcher;
        _objectStorage = objectStorage;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    [ProducesResponseType(
        typeof(FileUploadResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ValidationErrorResponse),
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload(
        [FromForm] UploadFileRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length <= 0)
        {
            return InvalidUploadRequest(
                nameof(request.File),
                "A valid file is required.");
        }

        if (request.File.Length > 10 * 1024 * 1024)
        {
            return InvalidUploadRequest(
                nameof(request.File),
                "Maximum file size is 10 MB.");
        }

        if (!Enum.IsDefined(request.Category))
        {
            return InvalidUploadRequest(
                nameof(request.Category),
                "Invalid file category.");
        }

        if (string.IsNullOrWhiteSpace(request.UploadedByUserId))
        {
            return InvalidUploadRequest(
                nameof(request.UploadedByUserId),
                "Uploader user ID is required.");
        }

        var file = request.File;
        var originalFileName = Path.GetFileName(file.FileName);

        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            return InvalidUploadRequest(
                nameof(request.File),
                "Invalid file name.");
        }

        var extension = Path.GetExtension(originalFileName);

        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".bin";
        }

        var contentType = string.IsNullOrWhiteSpace(file.ContentType)
            ? "application/octet-stream"
            : file.ContentType;

        string checksum;

        await using (var hashStream = file.OpenReadStream())
        {
            var hash = await SHA256.HashDataAsync(
                hashStream,
                cancellationToken);

            checksum = Convert.ToHexString(hash);
        }

        string? storageKey = null;
        var fileId = Guid.Empty;

        try
        {
            await using (var uploadStream = file.OpenReadStream())
            {
                storageKey = await _objectStorage.UploadAsync(
                    uploadStream,
                    originalFileName,
                    contentType,
                    cancellationToken);
            }

            fileId = await _dispatcher.SendAsync<
                RegisterFileCommand,
                Guid>(
                new RegisterFileCommand(
                    originalFileName,
                    storageKey,
                    contentType,
                    extension,
                    file.Length,
                    checksum,
                    request.Category.ToDomain(),
                    request.UploadedByUserId,
                    request.Description),
                cancellationToken);

            var completed = await _dispatcher.SendAsync<
                CompleteFileUploadCommand,
                bool>(
                new CompleteFileUploadCommand(fileId),
                cancellationToken);

            if (!completed)
            {
                throw new InvalidOperationException(
                    "File upload could not be completed.");
            }

            var response = new FileUploadResponse(
                fileId,
                originalFileName,
                file.Length,
                checksum,
                FileStatus.Available.ToString());

            return CreatedAtAction(
                nameof(GetById),
                new { fileId },
                response);
        }
        catch
        {
            if (fileId != Guid.Empty)
            {
                try
                {
                    await _dispatcher.SendAsync<DeleteFileCommand, bool>(
                        new DeleteFileCommand(
                            fileId,
                            request.UploadedByUserId),
                        CancellationToken.None);
                }
                catch
                {
                    if (!string.IsNullOrWhiteSpace(storageKey))
                    {
                        try
                        {
                            await _objectStorage.DeleteAsync(
                                storageKey,
                                CancellationToken.None);
                        }
                        catch
                        {
                            // Preserve the original exception.
                        }
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(storageKey))
            {
                try
                {
                    await _objectStorage.DeleteAsync(
                        storageKey,
                        CancellationToken.None);
                }
                catch
                {
                    // Preserve the original exception.
                }
            }

            throw;
        }
    }

    [HttpGet("{fileId:guid}")]
    [ProducesResponseType(
        typeof(FileResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid fileId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            GetFileByIdQuery,
            FileAssetDto?>(
            new GetFileByIdQuery(fileId),
            cancellationToken);

        return result is null
            ? NotFound()
            : Ok(result.ToResponse());
    }

    [HttpGet("{fileId:guid}/download")]
    [Produces("application/octet-stream")]
    [ProducesResponseType(
    typeof(FileStreamResult),
    StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(
        Guid fileId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            DownloadFileQuery,
            FileDownloadDto?>(
            new DownloadFileQuery(fileId),
            cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return File(
            result.Content,
            result.ContentType,
            result.FileName,
            enableRangeProcessing: true);
    }

    [HttpGet("by-reference/{referenceType}/{referenceId:guid}")]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<FileResponse>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByReference(
        FileReferenceTypeContract referenceType,
        Guid referenceId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            GetFilesByReferenceQuery,
            IReadOnlyCollection<FileAssetDto>>(
            new GetFilesByReferenceQuery(
                referenceType.ToDomain(),
                referenceId),
            cancellationToken);

        return Ok(result
            .Select(file => file.ToResponse())
            .ToArray());
    }

    [HttpGet("{fileId:guid}/references")]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<FileReferenceResponse>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReferences(
        Guid fileId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync<
            GetFileReferencesQuery,
            IReadOnlyCollection<FileReferenceDto>>(
            new GetFileReferencesQuery(fileId),
            cancellationToken);

        return Ok(result
            .Select(reference => reference.ToResponse())
            .ToArray());
    }

    [HttpPost("{fileId:guid}/references")]
    [ProducesResponseType(
        typeof(ResourceIdResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ValidationErrorResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ErrorResponse),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Link(
        Guid fileId,
        [FromBody] LinkFileRequest request,
        CancellationToken cancellationToken)
    {
        var referenceId = await _dispatcher.SendAsync<
            LinkFileCommand,
            Guid>(
            new LinkFileCommand(
                fileId,
                request.ReferenceType.ToDomain(),
                request.ReferenceId,
                request.LinkedByUserId,
                request.Purpose),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetReferences),
            new { fileId },
            new ResourceIdResponse(referenceId));
    }

    [HttpDelete("references/{referenceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unlink(
        Guid referenceId,
        CancellationToken cancellationToken)
    {
        var removed = await _dispatcher.SendAsync<
            UnlinkFileCommand,
            bool>(
            new UnlinkFileCommand(referenceId),
            cancellationToken);

        return removed ? NoContent() : NotFound();
    }

    [HttpPatch("{fileId:guid}/description")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ValidationErrorResponse),
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateDescription(
        Guid fileId,
        [FromBody] UpdateFileDescriptionRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<
            UpdateFileDescriptionCommand,
            bool>(
            new UpdateFileDescriptionCommand(
                fileId,
                request.Description),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{fileId:guid}/quarantine")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Quarantine(
        Guid fileId,
        [FromBody] FileReasonRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<
            QuarantineFileCommand,
            bool>(
            new QuarantineFileCommand(fileId, request.Reason),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{fileId:guid}/release")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReleaseFromQuarantine(
        Guid fileId,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<
            ReleaseFileFromQuarantineCommand,
            bool>(
            new ReleaseFileFromQuarantineCommand(fileId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{fileId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(
        Guid fileId,
        [FromBody] FileReasonRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _dispatcher.SendAsync<
            RejectFileCommand,
            bool>(
            new RejectFileCommand(fileId, request.Reason),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{fileId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
    Guid fileId,
    [FromQuery] string deletedByUserId,
    CancellationToken cancellationToken)
    {
        var deleted = await _dispatcher.SendAsync<
            DeleteFileCommand,
            bool>(
            new DeleteFileCommand(
                fileId,
                deletedByUserId),
            cancellationToken);

        return deleted ? NoContent() : NotFound();
    }

    private IActionResult InvalidUploadRequest(
    string field,
    string message)
    {
        return BadRequest(
            new ValidationErrorResponse(
                "VALIDATION_ERROR",
                "One or more validation errors occurred.",
                [
                    new ValidationError(field, message)
                ],
                HttpContext.TraceIdentifier));
    }
}
