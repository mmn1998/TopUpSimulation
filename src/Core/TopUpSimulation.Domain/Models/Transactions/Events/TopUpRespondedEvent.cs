using TopUpSimulation.Framework.Core.Entities;
using TopUpSimulation.Framework.Core.Events;

namespace TopUpSimulation.Domain.Models.Transactions.Events;

public class TopUpRespondedEvent : IIntegrationEvent
{
    public TopUpRespondedEvent(Guid correlationId, DateTime occurredOn, bool finishPayment)
    {
        CorrelationId = correlationId;
        OccurredOn = occurredOn;
        FinishPayment = finishPayment;
    }
    public Guid CorrelationId {  get; init; }
    public DateTime OccurredOn {  get; init; }
    public bool FinishPayment { get; init; }
}
