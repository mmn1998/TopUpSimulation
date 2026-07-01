using Microsoft.EntityFrameworkCore;
using Sima.Framework.Core.Repository;

namespace SIMA.Framework.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;

    public UnitOfWork(DbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
