using SIMA.Framework.Core.Entities;

namespace TopUpSimulation.Framework.Core.Contracts;

public interface IRepository<T> where T : Entity
{
    Task Add(T entity);
    void Remove(T entity);
}
