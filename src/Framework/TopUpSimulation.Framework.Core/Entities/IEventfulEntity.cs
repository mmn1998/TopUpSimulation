namespace TopUpSimulation.Framework.Core.Entities;

public interface IEventfulEntity
{
     void AddDomainEvent(IDomainEvent @event);

    void RemoveDomainEvent(IDomainEvent @event);

    void ClearDomainEvents();

    IReadOnlyCollection<IDomainEvent> GetDomainEvents();
}