using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sima.Framework.Core.Repository;
using TopUpSimulation.Framework.Core.Entities;

namespace SIMA.Framework.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private readonly IMediator _domainEventDispacher;

    public UnitOfWork(DbContext dbContext, IMediator domainEventDispacher)
    {
        this._context = dbContext;
        this._domainEventDispacher = domainEventDispacher;
    }

    public async Task<int> SaveChangesAsync()
    {
        var entitiesForSave = GetEntityForSave();
        var events = GetEvents(entitiesForSave);
        await RaiseEvent(events);
        return await _context.SaveChangesAsync();
    }
    public void Dispose()
    {
        this._context.Dispose();
    }

    private IList<IDomainEvent> GetEvents(IList<EntityEntry> entityForSave)
    {
        List<IDomainEvent> events = new List<IDomainEvent>();
        foreach (EntityEntry entityEntry in (IEnumerable<EntityEntry>)entityForSave)
        {
            if (entityEntry.Entity is IEventfulEntity entity && entity.GetDomainEvents() != null)
            {
                events.AddRange(entity.GetDomainEvents());
                entity.ClearDomainEvents();
            }
        }
        return events;
    }
    private async Task RaiseEvent(IList<IDomainEvent> events)
    {
        if (events != null)
        {
            foreach (var item in events)
            {
                await this._domainEventDispacher.Publish(item);
            }
        }
        await Task.CompletedTask;
    }

    private IList<EntityEntry> GetEntityForSave() =>
        this._context.ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Modified ||
                    x.State == EntityState.Added ||
                    x.State == EntityState.Deleted)
            .ToList();
}
