
using LIMS.Application.Instruments.Repositories;
using LIMS.Domain.Instruments;
using LIMS.Domain.Instruments.Enums;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Infrastructure.Persistence.Repositories;

public sealed class InstrumentRepository
    : GenericRepository<Instrument, Guid>, IInstrumentRepository
{
    public InstrumentRepository(LimsDbContext context)
        : base(context)
    {
    }

    public Task<bool> ExistsByCodeAsync(
        Guid laboratoryId,
        string instrumentCode,
        CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(
            x => x.LaboratoryId == laboratoryId &&
                 x.InstrumentCode == instrumentCode,
            cancellationToken);
    }

    public Task<Instrument?> GetWithDetailsAsync(
        Guid instrumentId,
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(x => x.MaintenanceRecords)
            .FirstOrDefaultAsync(
                x => x.Id == instrumentId,
                cancellationToken);
    }

    public Task<Instrument?> GetByCodeAsync(
        Guid laboratoryId,
        string instrumentCode,
        CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(
            x => x.LaboratoryId == laboratoryId &&
                 x.InstrumentCode == instrumentCode,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Instrument>> GetByLaboratoryIdAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => x.LaboratoryId == laboratoryId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Instrument>> GetByStatusAsync(
        Guid laboratoryId,
        InstrumentStatus status,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => x.LaboratoryId == laboratoryId &&
                        x.Status == status)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
