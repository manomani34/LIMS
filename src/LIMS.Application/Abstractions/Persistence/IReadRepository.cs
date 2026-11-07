namespace LIMS.Application.Abstractions.Persistence;

public interface IReadRepository<TEntity, in TId>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(
        TId id,
        CancellationToken cancellationToken = default);
}
