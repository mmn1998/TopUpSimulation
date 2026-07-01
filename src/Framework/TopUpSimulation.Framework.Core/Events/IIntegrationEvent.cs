namespace TopUpSimulation.Framework.Core.Events;

public interface IIntegrationEvent
{
    Guid CorrelationId { get; }
    DateTime OccurredOn { get; }
}