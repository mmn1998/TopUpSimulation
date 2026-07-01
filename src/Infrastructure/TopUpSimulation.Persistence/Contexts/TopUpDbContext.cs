using Microsoft.EntityFrameworkCore;
using TopUpSimulation.Domain.Models.Outboxes.Entities;
using TopUpSimulation.Domain.Models.Transactions.Entities;

namespace TopUpSimulation.Persistence.Contexts;

public class TopUpDbContext : DbContext
{
    public TopUpDbContext(DbContextOptions<TopUpDbContext> options) : base(options)
    {

    }
    public DbSet<TopUpOutBox> TopUpOutBoxes => Set<TopUpOutBox>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
}
