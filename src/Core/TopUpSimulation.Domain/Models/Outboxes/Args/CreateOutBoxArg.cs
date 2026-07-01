namespace TopUpSimulation.Domain.Models.Outboxes.Args;

public record CreateOutBoxArg(Guid correlationId, bool isProcessed, string topUpChargeRequest, DateTime occurredOn);
