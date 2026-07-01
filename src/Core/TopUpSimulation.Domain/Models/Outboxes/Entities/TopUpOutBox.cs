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
    }
    public static TopUpOutBox Create(CreateOutBoxArg arg)
    {
        return new TopUpOutBox(arg);
    }
    public Guid CorrelationId { get; init; }
    public bool IsProcessed { get; init; }
    public string TopUpChargeRequest { get; init; }
}
