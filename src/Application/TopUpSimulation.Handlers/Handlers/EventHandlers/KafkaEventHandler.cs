using TopUpSimulation.Domain.Models.Outboxes.Events;
using TopUpSimulation.Framework.Core.Events;

namespace TopUpSimulation.Handlers.Handlers.EventHandlers;

public class KafkaEventHandler : IEventHandler<ShaparakTransactionWaitingForConfirmEvent>
{
    public Task HandleAsync(ShaparakTransactionWaitingForConfirmEvent @event, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
