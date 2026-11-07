using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Common.Dispatching;

namespace LIMS.Application.Results.Commands;

public sealed record UpdateAnalysisResultInformationCommand(
    Guid ResultId,
    string Title,
    string? Description,
    Guid? InstrumentId) : ICommand<bool>;
