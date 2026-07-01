using Confluent.Kafka;
using System.Text.Json;
using TopUpSimulation.Framework.Core.Events;

namespace TopUpSimulation.Framework.Infrastructure.Publishers;

public class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<string, string> _producer;

    public KafkaEventPublisher(IProducer<string, string> producer)
    {
        _producer = producer;
    }

    public async Task PublishAsync<T>(
        T @event,
        CancellationToken cancellationToken)
        where T : IIntegrationEvent
    {
        var topic = typeof(T).Name;

        var json = JsonSerializer.Serialize(@event);

        await _producer.ProduceAsync(
            topic,
            new Message<string, string>
            {
                Key = @event.CorrelationId.ToString(),
                Value = json
            },
            cancellationToken);
    }
}
