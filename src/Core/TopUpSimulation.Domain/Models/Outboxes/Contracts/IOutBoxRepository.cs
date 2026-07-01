using TopUpSimulation.Domain.Models.Outboxes.Entities;
using TopUpSimulation.Framework.Core.Contracts;

namespace TopUpSimulation.Domain.Models.Outboxes.Contracts;

public interface IOutBoxRepository : IRepository<TopUpOutBox>
{
    Task<bool> CheckExist(Guid correlationId);
}
