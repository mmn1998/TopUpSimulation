using Microsoft.EntityFrameworkCore;
using SIMA.Framework.Infrastructure.Data;
using TopUpSimulation.Domain.Models.Outboxes.Contracts;
using TopUpSimulation.Domain.Models.Outboxes.Entities;
using TopUpSimulation.Persistence.Contexts;

namespace TopUpSimulation.Persistence.Repositories;

public class OutBoxRepository : Repository<TopUpOutBox>, IOutBoxRepository
{
    private readonly TopUpDbContext _context;

    public OutBoxRepository(TopUpDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> CheckExist(Guid correlationId)
    {
        return await _context.TopUpOutBoxes.AnyAsync(x => x.CorrelationId == correlationId);
    }
}