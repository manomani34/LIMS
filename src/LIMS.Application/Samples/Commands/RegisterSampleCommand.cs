using LIMS.Application.Abstractions.Messaging;
using LIMS.Domain.Samples.Enums;

namespace LIMS.Application.Samples.Commands;

public sealed record RegisterSampleCommand(
    Guid LaboratoryId,
    string SampleCode,
    string Name,
    string? Description,
    SampleType Type,
    SamplePriority Priority,
    string? Source,
    string RegisteredByUserId) : ICommand<Guid>;
