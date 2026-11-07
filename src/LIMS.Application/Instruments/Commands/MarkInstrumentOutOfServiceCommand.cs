using LIMS.Application.Abstractions.Messaging;

namespace LIMS.Application.Instruments.Commands;

public sealed record MarkInstrumentOutOfServiceCommand(
    Guid InstrumentId,
    string Reason) : ICommand<bool>;
