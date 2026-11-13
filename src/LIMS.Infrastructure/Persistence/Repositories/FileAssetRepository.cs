
using LIMS.Application.Files.Repositories;
using LIMS.Domain.Files.Entities;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Infrastructure.Persistence.Repositories;

public sealed class FileAssetRepository : IFileAssetRepository
{
    private readonly LimsDbContext _context;
    private readonly DbSet<FileAsset> _dbSet;

    public FileAssetRepository(LimsDbContext context)
    {
        _context = context;
        _dbSet = context.Set<FileAsset>();
    }

    public Task AddAsync(
        FileAsset file,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.AddAsync(file, cancellationToken).AsTask();
    }

    public Task<FileAsset?> GetByIdAsync(
        Guid fileId,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.FirstOrDefaultAsync(
            x => x.Id == fileId,
            cancellationToken);
    }

    public Task<bool> ExistsByStorageKeyAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.AnyAsync(
            x => x.StorageKey == storageKey,
            cancellationToken);
    }

    public void Update(FileAsset file)
    {
        _dbSet.Update(file);
    }
}
