namespace TopUpSimulation.Framework.Core.Events;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken)
        where T : IIntegrationEvent;
}