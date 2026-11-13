using LIMS.Domain.Files.Entities;

namespace LIMS.Application.Files.Repositories;

public interface IFileAssetRepository
{
    Task AddAsync(
        FileAsset file,
        CancellationToken cancellationToken = default);

    Task<FileAsset?> GetByIdAsync(
        Guid fileId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByStorageKeyAsync(
        string storageKey,
        CancellationToken cancellationToken = default);

    void Update(FileAsset file);
}
