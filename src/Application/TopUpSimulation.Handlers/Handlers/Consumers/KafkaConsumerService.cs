using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TopUpSimulation.Domain.Models.Outboxes.Events;
using TopUpSimulation.Domain.Models.Transactions.Events;
using TopUpSimulation.Handlers.Handlers.Dispatchers;

namespace TopUpSimulation.Handlers.Handlers.Consumers;

public class KafkaConsumerService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly IConsumer<string, string> _consumer;

    public KafkaConsumerService(
        IServiceProvider provider,
        IConsumer<string, string> consumer)
    {
        _provider = provider;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(new[]
        {
            nameof(ShaparakTransactionWaitingForConfirmEvent)
        });

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = _consumer.Consume(stoppingToken);

            using var scope = _provider.CreateScope();

            var dispatcher = scope.ServiceProvider
                .GetRequiredService<EventDispatcher>();

            await dispatcher.DispatchAsync(
                result.Topic,
                result.Message.Value,
                stoppingToken);
        }
    }
}
