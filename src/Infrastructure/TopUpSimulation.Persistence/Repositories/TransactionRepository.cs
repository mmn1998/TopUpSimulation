using SIMA.Framework.Infrastructure.Data;
using TopUpSimulation.Domain.Models.Transactions.Entities;
using TopUpSimulation.Persistence.Contexts;

namespace TopUpSimulation.Persistence.Repositories;

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    private readonly TopUpDbContext _context;

    public TransactionRepository(TopUpDbContext context) : base(context)
    {
        _context = context;
    }
}
