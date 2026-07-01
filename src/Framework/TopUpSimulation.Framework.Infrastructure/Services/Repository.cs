using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using TopUpSimulation.Framework.Core.Contracts;
using TopUpSimulation.Framework.Core.Entities;

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

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderby = null)
    {
        IQueryable<T> query = _dbSet;
        if (filter != null)
        {
            query = query.Where(filter);
        }
        if (orderby != null)
        {
            return orderby(query);
        }
        return query;
    }
}
