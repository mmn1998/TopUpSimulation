using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TopUpSimulation.Domain.Models.Outboxes.Events;
using TopUpSimulation.ExternalServices.TopUp;
using TopUpSimulation.Framework.Common.Exceptions;
using TopUpSimulation.Framework.Common.RestfulClient;
using TopUpSimulation.Framework.Common.Settings;
using TopUpSimulation.Framework.Core.Events;
using TopUpSimulation.Framework.Infrastructure.Publishers;
using TopUpSimulation.Handlers.Contracts.TopUp;
using TopUpSimulation.Handlers.Handlers.Consumers;
using TopUpSimulation.Handlers.Handlers.Dispatchers;
using TopUpSimulation.Handlers.Handlers.EventHandlers;

namespace TopUpSimulation.Handlers.Extensions;

public static class ConfigurationExtension
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaSetting = configuration.GetSection($"{nameof(KafkaSetting)}s").Get<KafkaSetting>() ?? throw TopUpResultException.ConfigureError;
        services.AddSingleton<IProducer<string, string>>(_ =>
        {
            var config = new Confluent.Kafka.ProducerConfig
            {
                BootstrapServers = kafkaSetting.ProducerConfigs.BootstrapServers
            };

            return new ProducerBuilder<string, string>(config).Build();
        });

        services.AddSingleton<IConsumer<string, string>>(_ =>
        {
            var config = new Confluent.Kafka.ConsumerConfig
            {
                BootstrapServers = kafkaSetting.ConsumerConfig.BootstrapServers,
                GroupId = kafkaSetting.ConsumerConfig.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            return new ConsumerBuilder<string, string>(config).Build();
        });

        services.AddSingleton<IEventPublisher, KafkaEventPublisher>();

        services.AddScoped<EventDispatcher>();

        services.AddScoped<IEventHandler<ShaparakTransactionWaitingForConfirmEvent>, KafkaEventHandler>();

        services.AddHostedService<KafkaConsumerService>();

        services.AddSingleton<IRestfulClient, RestfulClient>();
        services.AddSingleton<ITopUpService, MCITopUpService>();
        services.Configure<MCITopUpSetting>(configuration.GetSection($"{nameof(MCITopUpSetting)}s"));

        return services;
    }
}
