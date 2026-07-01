using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sima.Framework.Core.Repository;
using SIMA.Framework.Infrastructure.Data;
using TopUpSimulation.Domain.Models.Outboxes.Contracts;
using TopUpSimulation.Persistence.Repositories;

namespace TopUpSimulation.Persistence.Extensions;

public static class ConfigurationExtension
{
    public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IOutBoxRepository, OutBoxRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
