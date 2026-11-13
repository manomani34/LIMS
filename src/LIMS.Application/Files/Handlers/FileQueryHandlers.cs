using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Files.DTOs;
using LIMS.Application.Files.Mapping;
using LIMS.Application.Files.Queries;
using LIMS.Application.Files.Repositories;
using LIMS.Domain.Files.Enums;

namespace LIMS.Application.Files.Handlers;

public sealed class GetFileByIdQueryHandler
    : IQueryHandler<GetFileByIdQuery, FileAssetDto?>
{
    private readonly IFileAssetRepository _repository;

    public GetFileByIdQueryHandler(IFileAssetRepository repository)
    {
        _repository = repository;
    }

    public async Task<FileAssetDto?> HandleAsync(
        GetFileByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var file = await _repository.GetByIdAsync(
            query.FileId,
            cancellationToken);

        return file?.ToDto();
    }
}

public sealed class GetFilesByReferenceQueryHandler
    : IQueryHandler<
        GetFilesByReferenceQuery,
        IReadOnlyCollection<FileAssetDto>>
{
    private readonly IFileReferenceRepository _referenceRepository;
    private readonly IFileAssetRepository _fileRepository;

    public GetFilesByReferenceQueryHandler(
        IFileReferenceRepository referenceRepository,
        IFileAssetRepository fileRepository)
    {
        _referenceRepository = referenceRepository;
        _fileRepository = fileRepository;
    }

    public async Task<IReadOnlyCollection<FileAssetDto>> HandleAsync(
        GetFilesByReferenceQuery query,
        CancellationToken cancellationToken = default)
    {
        var references = await _referenceRepository.GetByReferenceAsync(
            query.ReferenceType,
            query.ReferenceId,
            cancellationToken);

        var files = new List<FileAssetDto>();

        foreach (var reference in references)
        {
            var file = await _fileRepository.GetByIdAsync(
                reference.FileId,
                cancellationToken);

            if (file is not null && file.Status != FileStatus.Deleted)
                files.Add(file.ToDto());
        }

        return files;
    }
}

public sealed class GetFileReferencesQueryHandler
    : IQueryHandler<
        GetFileReferencesQuery,
        IReadOnlyCollection<FileReferenceDto>>
{
    private readonly IFileReferenceRepository _repository;

    public GetFileReferencesQueryHandler(
        IFileReferenceRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<FileReferenceDto>> HandleAsync(
        GetFileReferencesQuery query,
        CancellationToken cancellationToken = default)
    {
        var references = await _repository.GetByFileIdAsync(
            query.FileId,
            cancellationToken);

        return references.Select(x => x.ToDto()).ToArray();
    }
}
