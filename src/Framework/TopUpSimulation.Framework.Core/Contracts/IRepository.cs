using System.Linq.Expressions;
using TopUpSimulation.Framework.Core.Entities;

namespace TopUpSimulation.Framework.Core.Contracts;

public interface IRepository<T> where T : Entity
{
    Task AddAsync(T entity);
    IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderby = null);
}
