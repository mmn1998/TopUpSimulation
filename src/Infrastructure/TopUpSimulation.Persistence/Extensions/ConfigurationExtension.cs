using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sima.Framework.Core.Repository;
using SIMA.Framework.Infrastructure.Data;
using System;
using TopUpSimulation.Domain.Models.Outboxes.Contracts;
using TopUpSimulation.Framework.Common.Exceptions;
using TopUpSimulation.Persistence.Contexts;
using TopUpSimulation.Persistence.Repositories;

namespace TopUpSimulation.Persistence.Extensions;

public static class ConfigurationExtension
{
    public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DbContext, TopUpDbContext>(options =>
        {
            options.UseInMemoryDatabase(configuration.GetValue<string>("InMemoryDbName") ?? throw TopUpResultException.ConfigureError);
        });
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IOutBoxRepository, OutBoxRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
