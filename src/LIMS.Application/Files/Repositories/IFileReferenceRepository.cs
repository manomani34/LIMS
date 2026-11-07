using LIMS.Domain.Files.Entities;
using LIMS.Domain.Files.Enums;

namespace LIMS.Application.Files.Repositories;

public interface IFileReferenceRepository
{
    Task AddAsync(
        FileReference reference,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid fileId,
        FileReferenceType referenceType,
        Guid referenceId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<FileReference>> GetByReferenceAsync(
        FileReferenceType referenceType,
        Guid referenceId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<FileReference>> GetByFileIdAsync(
        Guid fileId,
        CancellationToken cancellationToken = default);

    Task<FileReference?> GetAsync(
        Guid referenceId,
        CancellationToken cancellationToken = default);

    void Remove(FileReference reference);
}
