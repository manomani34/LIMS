
using LIMS.Application.Results.Repositories;
using LIMS.Domain.Results;
using LIMS.Domain.Results.Enums;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Infrastructure.Persistence.Repositories;

public sealed class AnalysisResultRepository
    : GenericRepository<AnalysisResult, Guid>, IAnalysisResultRepository
{
    public AnalysisResultRepository(LimsDbContext context)
        : base(context)
    {
    }

    public Task<bool> ExistsByCodeAsync(
        Guid laboratoryId,
        string resultCode,
        CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(
            x => x.LaboratoryId == laboratoryId
                 && x.ResultCode == resultCode,
            cancellationToken);
    }

    public Task<AnalysisResult?> GetWithDetailsAsync(
        Guid resultId,
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(x => x.Entries)
            .FirstOrDefaultAsync(
                x => x.Id == resultId,
                cancellationToken);
    }

    public Task<AnalysisResult?> GetByCodeAsync(
        Guid laboratoryId,
        string resultCode,
        CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(
            x => x.LaboratoryId == laboratoryId
                 && x.ResultCode == resultCode,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<AnalysisResult>> GetByLaboratoryIdAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.LaboratoryId == laboratoryId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AnalysisResult>> GetByExperimentIdAsync(
        Guid experimentId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.ExperimentId == experimentId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AnalysisResult>> GetBySampleIdAsync(
        Guid sampleId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.SampleId == sampleId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AnalysisResult>> GetByStatusAsync(
        Guid laboratoryId,
        ResultStatus status,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.LaboratoryId == laboratoryId
                        && x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
