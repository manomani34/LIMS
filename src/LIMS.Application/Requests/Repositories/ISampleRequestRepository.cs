using LIMS.Application.Abstractions.Persistence;
using LIMS.Domain.Requests;
using LIMS.Domain.Requests.Enums;

namespace LIMS.Application.Requests.Repositories;

public interface ISampleRequestRepository
    : IRepository<SampleRequest, Guid>
{
    Task<bool> ExistsByCodeAsync(
        string requestCode,
        CancellationToken cancellationToken = default);

    Task<SampleRequest?> GetByCodeAsync(
        string requestCode,
        CancellationToken cancellationToken = default);

    Task<SampleRequest?> GetWithDetailsAsync(
        Guid requestId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SampleRequest>> GetByRequesterAsync(
        string requestedByUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SampleRequest>> GetByLaboratoryIdAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SampleRequest>> GetByStatusAsync(
        SampleRequestStatus status,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SampleRequest>> GetPendingReviewAsync(
        CancellationToken cancellationToken = default);
}
