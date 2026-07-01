namespace TopUpSimulation.Framework.Core.Entities;

public abstract class Entity : IEventfulEntity, IComparable, IComparable<Entity>
{
    private List<IDomainEvent> _domainEvents;
    protected Entity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.Now;
        _domainEvents = new List<IDomainEvent>();
    }
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public override bool Equals(object obj)
    {
        if (!(obj is Entity other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        
        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(Entity a, Entity b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity a, Entity b)
    {
        return !(a == b);
    }

    public virtual int CompareTo(Entity other)
    {
        if (other is null)
            return 1;

        if (ReferenceEquals(this, other))
            return 0;

        return 0;
    }

    public virtual int CompareTo(object other)
    {
        return CompareTo(other as Entity);
    }

    public void AddDomainEvent(IDomainEvent @event)
    {
        _domainEvents?.Add(@event);
    }

    public void RemoveDomainEvent(IDomainEvent @event)
    {
        _domainEvents?.Remove(@event);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }

    public IReadOnlyCollection<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents?.AsReadOnly();
    }
}
