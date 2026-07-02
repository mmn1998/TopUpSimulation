using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TopUpSimulation.Domain.Models.Outboxes.Events;
using TopUpSimulation.Framework.Common.Exceptions;
using TopUpSimulation.Handlers.Handlers.Dispatchers;

namespace TopUpSimulation.Handlers.Handlers.Consumers;

public class KafkaConsumerService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly IConsumer<string, string> _consumer;
    private readonly int _kafkaTimeout;

    public KafkaConsumerService(
        IServiceProvider provider,
        IConsumer<string, string> consumer,
        IConfiguration configuration)
    {
        _provider = provider;
        _consumer = consumer;
        _kafkaTimeout = configuration.GetValue<int?>("KafkaSettings:Timeout") ?? throw TopUpResultException.ConfigureError;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _consumer.Subscribe(new[]
            {
                nameof(ShaparakTransactionWaitingForConfirmEvent)
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(_kafkaTimeout);

                using var scope = _provider.CreateScope();

                var dispatcher = scope.ServiceProvider
                    .GetRequiredService<EventDispatcher>();

                await dispatcher.DispatchAsync(
                    result.Topic,
                    result.Message.Value,
                    stoppingToken);
            }
        }
        catch (Exception)
        {            
            throw TopUpResultException.BrokerError;
        }
    }
}
