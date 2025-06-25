using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;
using TelematicsCore.Interfaces;
using TelematicsCore.Services;
using TelematicsData;


var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/telematics-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure JSON serialization for .NET 9.0 compatibility
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        
        // Fix for .NET 9.0 PipeWriter compatibility
        options.JsonSerializerOptions.WriteIndented = false;
        options.JsonSerializerOptions.AllowTrailingCommas = true;
        options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Telematics Data Platform API",
        Version = "v1",
        Description = "API for processing vehicle telematics data"
    });
});

// Configure Entity Framework
builder.Services.AddDbContext<TelematicsDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.UseNetTopologySuite()));

// Register DbContext for dependency injection in services
builder.Services.AddScoped<DbContext>(provider => provider.GetService<TelematicsDbContext>()!);

// Register services
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<ITelematicsEventService, TelematicsEventService>();
builder.Services.AddScoped<IDataEnrichmentService, DataEnrichmentService>();

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<TelematicsDbContext>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

// Map health checks
app.MapHealthChecks("/api/health");

app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TelematicsDbContext>();
    await context.Database.EnsureCreatedAsync();
}

app.Run();
public partial class Program { }
