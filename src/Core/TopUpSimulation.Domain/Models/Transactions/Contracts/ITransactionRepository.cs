using TopUpSimulation.Domain.Models.Transactions.Entities;
using TopUpSimulation.Framework.Core.Contracts;

namespace TopUpSimulation.Persistence.Repositories;

public interface ITransactionRepository : IRepository<Transaction>
{
}