namespace TopUpSimulation.Framework.Core.Events;

public interface IEventHandler<in T>
    where T : IIntegrationEvent
{
    Task HandleAsync(T @event, CancellationToken cancellationToken);
}