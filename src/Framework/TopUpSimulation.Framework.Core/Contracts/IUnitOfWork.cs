namespace Sima.Framework.Core.Repository;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
