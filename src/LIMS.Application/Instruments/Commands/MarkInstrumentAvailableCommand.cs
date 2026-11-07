using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Instruments.Commands;

public sealed record MarkInstrumentAvailableCommand(
    Guid InstrumentId) : ICommand<bool>;
