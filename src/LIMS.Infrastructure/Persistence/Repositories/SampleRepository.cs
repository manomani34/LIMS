
using LIMS.Application.Samples.Interfaces;
using LIMS.Domain.Samples;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Infrastructure.Persistence.Repositories;

public sealed class SampleRepository
    : GenericRepository<Sample, Guid>, ISampleRepository
{
    public SampleRepository(LimsDbContext context)
        : base(context)
    {
    }

    public Task<bool> ExistsByCodeAsync(
        Guid laboratoryId,
        string sampleCode,
        CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(
            x => x.LaboratoryId == laboratoryId
                 && x.SampleCode == sampleCode,
            cancellationToken);
    }

    public Task<Sample?> GetByCodeAsync(
        Guid laboratoryId,
        string sampleCode,
        CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(
            x => x.LaboratoryId == laboratoryId
                 && x.SampleCode == sampleCode,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Sample>> GetByLaboratoryIdAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.LaboratoryId == laboratoryId)
            .OrderByDescending(x => x.RegisteredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Sample>> GetByAssignedUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.AssignedToUserId == userId)
            .OrderByDescending(x => x.RegisteredAt)
            .ToListAsync(cancellationToken);
    }

    public Task<Sample?> GetWithDetailsAsync(
        Guid sampleId,
        CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(
            x => x.Id == sampleId,
            cancellationToken);
    }
}
