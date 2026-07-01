using Microsoft.EntityFrameworkCore;
using SIMA.Framework.Core.Entities;
using TopUpSimulation.Framework.Core.Contracts;

namespace SIMA.Framework.Infrastructure.Data;

public class Repository<T> : IRepository<T> where T : Entity
{
    protected DbContext Context;
    private protected DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        Context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task Add(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
}
