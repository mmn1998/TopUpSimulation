using TopUpSimulation.Domain.Models.Outboxes.Args;
using TopUpSimulation.Framework.Core.Entities;

namespace TopUpSimulation.Domain.Models.Outboxes.Entities;

public class TopUpOutBox : Entity
{
    private TopUpOutBox()
    {
        
    }
    private TopUpOutBox(CreateOutBoxArg arg)
    {
        CorrelationId = arg.correlationId;
        IsProcessed = arg.isProcessed;
        TopUpChargeRequest = arg.topUpChargeRequest;
        OccurredOn = arg.occurredOn;
    }
    public static TopUpOutBox Create(CreateOutBoxArg arg)
    {
        return new TopUpOutBox(arg);
    }
    public void ProcessedCompleted()
    {
        IsProcessed = true;
    }
    public Guid CorrelationId { get; init; }
    public bool IsProcessed { get; private set; }
    public string TopUpChargeRequest { get; init; }
    public DateTime OccurredOn { get; init; }
}
