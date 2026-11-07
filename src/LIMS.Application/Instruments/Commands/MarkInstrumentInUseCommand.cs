using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Instruments.Commands;

public sealed record MarkInstrumentInUseCommand(
    Guid InstrumentId) : ICommand<bool>;
