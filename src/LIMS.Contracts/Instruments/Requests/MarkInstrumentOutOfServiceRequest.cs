namespace LIMS.Contracts.Instruments.Requests;

public sealed record MarkInstrumentOutOfServiceRequest(
    string Reason);
