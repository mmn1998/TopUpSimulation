using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sima.Framework.Core.Repository;
using TopUpSimulation.Domain.Models.Outboxes.Contracts;
using TopUpSimulation.Domain.Models.Transactions.Args;
using TopUpSimulation.Domain.Models.Transactions.Entities;
using TopUpSimulation.Domain.Models.Transactions.Events;
using TopUpSimulation.Framework.Common.Exceptions;
using TopUpSimulation.Framework.Core.Events;
using TopUpSimulation.Handlers.Contracts.TopUp;
using TopUpSimulation.Handlers.Contracts.TopUp.Requests;
using TopUpSimulation.Persistence.Repositories;

namespace TopUpSimulation.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IOutBoxRepository _outBoxRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITopUpService _topUpService;
    private readonly IEventPublisher _eventPublisher;

    public Worker(ILogger<Worker> logger, ITransactionRepository transactionRepository, IOutBoxRepository outBoxRepository,
        IUnitOfWork unitOfWork, ITopUpService topUpService, IEventPublisher eventPublisher)
    {
        _logger = logger;
        _transactionRepository = transactionRepository;
        _outBoxRepository = outBoxRepository;
        _unitOfWork = unitOfWork;
        _topUpService = topUpService;
        _eventPublisher = eventPublisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            var remainedEvents = await _outBoxRepository.GetAll(x => !x.IsProcessed).ToListAsync();
            _logger.LogInformation($"{remainedEvents.Count} events found");
            foreach (var remainedEvent in remainedEvents)
            {
                try
                {
                    _logger.LogInformation($"charge request is : {remainedEvent.TopUpChargeRequest}");
                    var topUpChargeRequest = JsonConvert.DeserializeObject<InstantChargeRequest>(remainedEvent.TopUpChargeRequest);
                    if (topUpChargeRequest == null) throw TopUpResultException.DeserializeError;

                    var chargeResponse = await _topUpService.InstantCharge(topUpChargeRequest);
                    if (chargeResponse == null) throw TopUpResultException.TopUpChargeError;
                    _logger.LogInformation($"charge service called and the response is : {JsonConvert.SerializeObject(chargeResponse)}");
                    #region add transaction
                    var transactionArg = new CreateTransactionArg(request: remainedEvent.TopUpChargeRequest,
                        response: JsonConvert.SerializeObject(chargeResponse), successfull: chargeResponse.isSuccessful);
                    var transaction = Transaction.Create(transactionArg);
                    await _transactionRepository.AddAsync(transaction);
                    _logger.LogInformation($"transaction with value : {JsonConvert.SerializeObject(transaction)} has created");
                    #endregion
                    #region publish event
                    var @event = new TopUpRespondedEvent(correlationId: transaction.Id,
                        occurredOn: transaction.CreatedAt, finishPayment: chargeResponse.isSuccessful);
                    _eventPublisher.PublishAsync(@event, stoppingToken);
                    _logger.LogInformation($"event with value : {JsonConvert.SerializeObject(@event)} has published on kafka");
                    #endregion
                    #region final update
                    remainedEvent.ProcessedCompleted();
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"all changes commited to database");
                    #endregion
                }
                catch (Exception e)
                {
                    throw new TopUpException(e.Message, e);
                }

            }
        }
    }
}
