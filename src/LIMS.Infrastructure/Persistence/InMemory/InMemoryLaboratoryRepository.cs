using LIMS.Application.Laboratories.Interfaces;
using LIMS.Domain.Laboratories;

namespace LIMS.Infrastructure.Persistence.InMemory;

public sealed class InMemoryLaboratoryRepository
    : ILaboratoryRepository
{
    private readonly InMemoryLimsStore _store;

    public InMemoryLaboratoryRepository(InMemoryLimsStore store)
    {
        _store = store;
    }

    public Task<Laboratory?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(_store.GetLaboratory(id));
    }

    public Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var exists = _store.GetLaboratories().Any(x =>
            string.Equals(
                x.Code,
                code,
                StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(exists);
    }

    public Task<Laboratory?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var laboratory = _store.GetLaboratories().FirstOrDefault(x =>
            string.Equals(
                x.Code,
                code,
                StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(laboratory);
    }

    public Task<Laboratory?> GetWithMembersAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(
            _store.GetLaboratory(laboratoryId));
    }

    public Task<IReadOnlyCollection<Laboratory>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyCollection<Laboratory> laboratories = _store
            .GetLaboratories()
            .Where(x => x.Members.Any(member =>
                string.Equals(
                    member.UserId,
                    userId,
                    StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        return Task.FromResult(laboratories);
    }

    public Task AddAsync(
        Laboratory entity,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _store.AddLaboratory(entity);

        return Task.CompletedTask;
    }

    public void Update(Laboratory entity)
    {
        _store.UpdateLaboratory(entity);
    }

    public void Remove(Laboratory entity)
    {
        _store.RemoveLaboratory(entity);
    }
}
