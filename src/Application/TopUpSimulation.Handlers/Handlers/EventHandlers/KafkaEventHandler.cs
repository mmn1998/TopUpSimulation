using Microsoft.Extensions.Logging;
using Sima.Framework.Core.Repository;
using System.Text.Json;
using TopUpSimulation.Domain.Models.Outboxes.Args;
using TopUpSimulation.Domain.Models.Outboxes.Contracts;
using TopUpSimulation.Domain.Models.Outboxes.Entities;
using TopUpSimulation.Domain.Models.Outboxes.Events;
using TopUpSimulation.Framework.Common.Exceptions;
using TopUpSimulation.Framework.Core.Events;

namespace TopUpSimulation.Handlers.Handlers.EventHandlers;

public class KafkaEventHandler : IEventHandler<ShaparakTransactionWaitingForConfirmEvent>
{
    private readonly IOutBoxRepository _outBoxRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<KafkaEventHandler> _logger;

    public KafkaEventHandler(IOutBoxRepository outBoxRepository,
        IUnitOfWork unitOfWork, ILogger<KafkaEventHandler> logger)
    {
        _outBoxRepository = outBoxRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task HandleAsync(ShaparakTransactionWaitingForConfirmEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"event {nameof(ShaparakTransactionWaitingForConfirmEvent)}" +
                $" arrived and handled with correlationId : {@event.CorrelationId} and value : {JsonSerializer.Serialize(@event)}");

            if (await _outBoxRepository.CheckExist(@event.CorrelationId))
            {
                _logger.LogInformation($"correlationId {@event.CorrelationId} was already inserted into db");
            }
            else
            {
                _logger.LogInformation($"correlationId {@event.CorrelationId} was a new event");
                var outboxArg = new CreateOutBoxArg(correlationId: @event.CorrelationId,
                    isProcessed: false, topUpChargeRequest: @event.TopUpChargeRequest, occurredOn: @event.OccurredOn);
                var entity = TopUpOutBox.Create(outboxArg);
                await _outBoxRepository.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"correlationId {@event.CorrelationId} proccessed succesfully");
                
            }
        }
        catch (Exception e)
        {
            throw new TopUpException(e.Message, e);
        }
    }
}
