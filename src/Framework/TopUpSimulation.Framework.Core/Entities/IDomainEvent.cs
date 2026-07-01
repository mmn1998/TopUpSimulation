using MediatR;

namespace TopUpSimulation.Framework.Core.Entities;
public interface IDomainEvent : INotification
{
    Guid CorrelationId { get; }
    DateTime CreatedAt { get; }
}