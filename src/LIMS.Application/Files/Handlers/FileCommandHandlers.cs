using LIMS.Application.Abstractions.Persistence;
using LIMS.Application.Abstractions.Storage;
using LIMS.Application.Abstractions.Time;
using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Files.Commands;
using LIMS.Application.Files.Repositories;
using LIMS.Domain.Files.Entities;
using LIMS.Domain.Files.Enums;
using LIMS.Domain.Files;

namespace LIMS.Application.Files.Handlers;

public sealed class RegisterFileCommandHandler
    : ICommandHandler<RegisterFileCommand, Guid>
{
    private readonly IFileAssetRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RegisterFileCommandHandler(
        IFileAssetRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Guid> HandleAsync(
        RegisterFileCommand command,
        CancellationToken cancellationToken = default)
    {
        if (await _repository.ExistsByStorageKeyAsync(
                command.StorageKey,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "A file with this storage key already exists.");
        }

        var file = FileAsset.Register(
            command.OriginalFileName,
            command.StorageKey,
            command.ContentType,
            command.FileExtension,
            command.SizeInBytes,
            command.Sha256Checksum,
            command.Category,
            command.UploadedByUserId,
            command.Description,
            _dateTimeProvider.UtcNow);

        await _repository.AddAsync(file, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return file.Id;
    }
}

public sealed class CompleteFileUploadCommandHandler
    : ICommandHandler<CompleteFileUploadCommand, bool>
{
    private readonly IFileAssetRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CompleteFileUploadCommandHandler(
        IFileAssetRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        CompleteFileUploadCommand command,
        CancellationToken cancellationToken = default)
    {
        var file = await _repository.GetByIdAsync(
            command.FileId,
            cancellationToken);

        if (file is null)
            return false;

        file.MarkAvailable(_dateTimeProvider.UtcNow);

        _repository.Update(file);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public sealed class QuarantineFileCommandHandler
    : ICommandHandler<QuarantineFileCommand, bool>
{
    private readonly IFileAssetRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public QuarantineFileCommandHandler(
        IFileAssetRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        QuarantineFileCommand command,
        CancellationToken cancellationToken = default)
    {
        var file = await _repository.GetByIdAsync(
            command.FileId,
            cancellationToken);

        if (file is null)
            return false;

        file.Quarantine(command.Reason, _dateTimeProvider.UtcNow);

        _repository.Update(file);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public sealed class ReleaseFileFromQuarantineCommandHandler
    : ICommandHandler<ReleaseFileFromQuarantineCommand, bool>
{
    private readonly IFileAssetRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ReleaseFileFromQuarantineCommandHandler(
        IFileAssetRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        ReleaseFileFromQuarantineCommand command,
        CancellationToken cancellationToken = default)
    {
        var file = await _repository.GetByIdAsync(
            command.FileId,
            cancellationToken);

        if (file is null)
            return false;

        file.ReleaseFromQuarantine(_dateTimeProvider.UtcNow);

        _repository.Update(file);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public sealed class RejectFileCommandHandler
    : ICommandHandler<RejectFileCommand, bool>
{
    private readonly IFileAssetRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectFileCommandHandler(
        IFileAssetRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(
        RejectFileCommand command,
        CancellationToken cancellationToken = default)
    {
        var file = await _repository.GetByIdAsync(
            command.FileId,
            cancellationToken);

        if (file is null)
            return false;

        file.Reject(command.Reason);

        _repository.Update(file);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public sealed class DeleteFileCommandHandler
    : ICommandHandler<DeleteFileCommand, bool>
{
    private readonly IFileAssetRepository _repository;
    private readonly IObjectStorage _objectStorage;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DeleteFileCommandHandler(
        IFileAssetRepository repository,
        IObjectStorage objectStorage,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _objectStorage = objectStorage;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        DeleteFileCommand command,
        CancellationToken cancellationToken = default)
    {
        var file = await _repository.GetByIdAsync(
            command.FileId,
            cancellationToken);

        if (file is null)
            return false;

        file.Delete(
            command.DeletedByUserId,
            _dateTimeProvider.UtcNow);

        _repository.Update(file);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _objectStorage.DeleteAsync(
            file.StorageKey,
            cancellationToken);

        return true;
    }
}

public sealed class UpdateFileDescriptionCommandHandler
    : ICommandHandler<UpdateFileDescriptionCommand, bool>
{
    private readonly IFileAssetRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFileDescriptionCommandHandler(
        IFileAssetRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(
        UpdateFileDescriptionCommand command,
        CancellationToken cancellationToken = default)
    {
        var file = await _repository.GetByIdAsync(
            command.FileId,
            cancellationToken);

        if (file is null)
            return false;

        file.UpdateDescription(command.Description);

        _repository.Update(file);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public sealed class LinkFileCommandHandler
    : ICommandHandler<LinkFileCommand, Guid>
{
    private readonly IFileAssetRepository _fileRepository;
    private readonly IFileReferenceRepository _referenceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LinkFileCommandHandler(
        IFileAssetRepository fileRepository,
        IFileReferenceRepository referenceRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _fileRepository = fileRepository;
        _referenceRepository = referenceRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Guid> HandleAsync(
        LinkFileCommand command,
        CancellationToken cancellationToken = default)
    {
        var file = await _fileRepository.GetByIdAsync(
            command.FileId,
            cancellationToken);

        if (file is null)
            throw new FileDomainException("File was not found.");

        if (file.Status != FileStatus.Available)
            throw new FileDomainException(
    "Only available files can be linked.");

        if (await _referenceRepository.ExistsAsync(
                command.FileId,
                command.ReferenceType,
                command.ReferenceId,
                cancellationToken))
        {
            throw new FileDomainException(
    "This file is already linked to the specified resource.");
        }

        var reference = new FileReference(
            command.FileId,
            command.ReferenceType,
            command.ReferenceId,
            command.LinkedByUserId,
            command.Purpose,
            _dateTimeProvider.UtcNow);

        await _referenceRepository.AddAsync(reference, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return reference.Id;
    }
}


public sealed class UnlinkFileCommandHandler
    : ICommandHandler<UnlinkFileCommand, bool>
{
    private readonly IFileReferenceRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UnlinkFileCommandHandler(
        IFileReferenceRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(
        UnlinkFileCommand command,
        CancellationToken cancellationToken = default)
    {
        var reference = await _repository.GetAsync(
            command.FileReferenceId,
            cancellationToken);

        if (reference is null)
            return false;

        reference.Unlink(_dateTimeProvider.UtcNow);

        _repository.Remove(reference);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

