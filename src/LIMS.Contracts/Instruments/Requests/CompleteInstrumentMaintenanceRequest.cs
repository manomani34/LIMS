namespace LIMS.Contracts.Instruments.Requests;

public sealed record CompleteInstrumentMaintenanceRequest(
    string CompletedByUserId,
    string? CompletionNotes);
