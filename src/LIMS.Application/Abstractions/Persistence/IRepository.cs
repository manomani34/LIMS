namespace LIMS.Application.Abstractions.Persistence;

public interface IRepository<TEntity, in TId> : IReadRepository<TEntity, TId>
    where TEntity : class
{
    Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    void Update(TEntity entity);

    void Remove(TEntity entity);
}
