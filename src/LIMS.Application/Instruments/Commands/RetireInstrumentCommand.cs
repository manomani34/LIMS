using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Instruments.Commands;

public sealed record RetireInstrumentCommand(
    Guid InstrumentId) : ICommand<bool>;
