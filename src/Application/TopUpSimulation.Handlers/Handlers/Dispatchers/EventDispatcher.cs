using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using TopUpSimulation.Domain.Models.Transactions.Events;
using TopUpSimulation.Framework.Core.Events;

namespace TopUpSimulation.Handlers.Handlers.Dispatchers;

public class EventDispatcher
{
    private readonly IServiceProvider _provider;

    public EventDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task DispatchAsync(
        string topic,
        string json,
        CancellationToken token)
    {
        switch (topic)
        {
            case nameof(TopUpRespondedEvent):

                var evt =
                    JsonSerializer.Deserialize<TopUpRespondedEvent>(json)!;

                var handlers =
                    _provider.GetServices<IEventHandler<TopUpRespondedEvent>>();

                foreach (var handler in handlers)
                {
                    await handler.HandleAsync(evt, token);
                }

                break;
        }
    }
}
