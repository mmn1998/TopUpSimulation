using Serilog;
using TopUpSimulation.Handlers.Extensions;
using TopUpSimulation.Persistence.Extensions;
using TopUpSimulation.Worker;

var builder = Host.CreateApplicationBuilder(args);
#region configuring serilog

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
#endregion
#region Service registrations

builder.Services
    .AddSerilog()
    .RegisterApplicationServices(builder.Configuration)
    .RegisterInfrastructureServices(builder.Configuration)
    .AddHostedService<Worker>();
#endregion

#region run app

var host = builder.Build();
try
{
    Log.Information("Worker starting");

    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
#endregion
