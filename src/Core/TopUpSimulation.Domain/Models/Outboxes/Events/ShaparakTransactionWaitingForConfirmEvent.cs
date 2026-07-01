using System;
using TopUpSimulation.Framework.Core.Events;

namespace TopUpSimulation.Domain.Models.Outboxes.Events
{
    public class ShaparakTransactionWaitingForConfirmEvent : IIntegrationEvent
    {
        public Guid CorrelationId { get; }
        public DateTime OccurredOn { get; }
        public string TopUpChargeRequest { get; }
    }
}
