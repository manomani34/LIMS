using LIMS.Application.Abstractions.Persistence;
using LIMS.Domain.Results;
using LIMS.Domain.Results.Enums;

namespace LIMS.Application.Results.Repositories;

public interface IAnalysisResultRepository : IRepository<AnalysisResult, Guid>
{
    Task<bool> ExistsByCodeAsync(
        Guid laboratoryId,
        string resultCode,
        CancellationToken cancellationToken = default);

    Task<AnalysisResult?> GetWithDetailsAsync(
        Guid resultId,
        CancellationToken cancellationToken = default);

    Task<AnalysisResult?> GetByCodeAsync(
        Guid laboratoryId,
        string resultCode,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AnalysisResult>> GetByLaboratoryIdAsync(
        Guid laboratoryId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AnalysisResult>> GetByExperimentIdAsync(
        Guid experimentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AnalysisResult>> GetBySampleIdAsync(
        Guid sampleId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AnalysisResult>> GetByStatusAsync(
        Guid laboratoryId,
        ResultStatus status,
        CancellationToken cancellationToken = default);
}
