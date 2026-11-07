using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Instruments.Commands;

public sealed record RestoreInstrumentServiceCommand(
    Guid InstrumentId) : ICommand<bool>;
