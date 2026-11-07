using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Laboratories.DTOs;
using LIMS.Application.Laboratories.Interfaces;
using LIMS.Application.Laboratories.Queries;

namespace LIMS.Application.Laboratories.Handlers;

public sealed class GetLaboratoriesByUserIdQueryHandler
    : IQueryHandler<
        GetLaboratoriesByUserIdQuery,
        IReadOnlyCollection<LaboratoryDto>>
{
    private readonly ILaboratoryRepository _laboratoryRepository;

    public GetLaboratoriesByUserIdQueryHandler(
        ILaboratoryRepository laboratoryRepository)
    {
        _laboratoryRepository = laboratoryRepository;
    }

    public async Task<IReadOnlyCollection<LaboratoryDto>> HandleAsync(
        GetLaboratoriesByUserIdQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (string.IsNullOrWhiteSpace(query.UserId))
            throw new ArgumentException("User identifier is required.");

        var laboratories = await _laboratoryRepository.GetByUserIdAsync(
            query.UserId.Trim(),
            cancellationToken);

        return laboratories
            .Select(laboratory => new LaboratoryDto(
                laboratory.Id,
                laboratory.Name,
                laboratory.Code,
                laboratory.Description,
                laboratory.Status,
                laboratory.CreatedAt,
                laboratory.UpdatedAt,
                laboratory.Members.Count))
            .ToArray();
    }
}
