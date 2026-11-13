using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Results.Commands;

public sealed record CreateAnalysisResultCommand(
    Guid LaboratoryId,
    Guid ExperimentId,
    Guid SampleId,
    Guid? InstrumentId,
    string ResultCode,
    string Title,
    string? Description,
    string CreatedByUserId) : ICommand<Guid>;

