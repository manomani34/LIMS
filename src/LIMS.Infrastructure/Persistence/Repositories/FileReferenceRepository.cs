
using LIMS.Application.Files.Repositories;
using LIMS.Domain.Files.Entities;
using LIMS.Domain.Files.Enums;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Infrastructure.Persistence.Repositories;

public sealed class FileReferenceRepository : IFileReferenceRepository
{
    private readonly DbSet<FileReference> _dbSet;

    public FileReferenceRepository(LimsDbContext context)
    {
        _dbSet = context.Set<FileReference>();
    }

    public Task AddAsync(
        FileReference reference,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.AddAsync(reference, cancellationToken).AsTask();
    }

    public Task<bool> ExistsAsync(
        Guid fileId,
        FileReferenceType referenceType,
        Guid referenceId,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.AnyAsync(
            x => x.FileId == fileId
                 && x.ReferenceType == referenceType
                 && x.ReferenceId == referenceId,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<FileReference>> GetByReferenceAsync(
        FileReferenceType referenceType,
        Guid referenceId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.ReferenceType == referenceType
                        && x.ReferenceId == referenceId)
            .OrderByDescending(x => x.LinkedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<FileReference>> GetByFileIdAsync(
        Guid fileId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.FileId == fileId)
            .OrderByDescending(x => x.LinkedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<FileReference?> GetAsync(
        Guid referenceId,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.FirstOrDefaultAsync(
            x => x.Id == referenceId,
            cancellationToken);
    }

    public void Remove(FileReference reference)
    {
        _dbSet.Remove(reference);
    }
}
