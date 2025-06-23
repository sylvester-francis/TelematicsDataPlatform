using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TelematicsCore.Interfaces;
using TelematicsCore.Services;
using TelematicsData;
using TelematicsBatchProcessor.Services;


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// Fix: Use correct Serilog configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/batch-processor-.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();

try
{
    Log.Information("Starting Telematics Batch Processor");

    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureServices((context, services) =>
        {
            // Configure Entity Framework
            services.AddDbContext<TelematicsDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    x => x.UseNetTopologySuite()));

            // Register DbContext for dependency injection in services
            services.AddScoped<DbContext>(provider => provider.GetService<TelematicsDbContext>()!);

            // Register services
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<ITelematicsEventService, TelematicsEventService>();
            services.AddScoped<IDataEnrichmentService, DataEnrichmentService>();

            // Register background service
            services.AddHostedService<BatchProcessingService>();
        })
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}