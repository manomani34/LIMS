
using LIMS.Application.Abstractions.Persistence;
using LIMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Infrastructure.Persistence.Repositories;

public class GenericRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : class
{
    protected readonly LimsDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public GenericRepository(LimsDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        TId id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(
            [id],
            cancellationToken);
    }

    public virtual async Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public virtual void Remove(TEntity entity)
    {
        DbSet.Remove(entity);
    }
}
