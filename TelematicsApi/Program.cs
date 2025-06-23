using Microsoft.EntityFrameworkCore;
using Serilog;
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
builder.Services.AddControllers();
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
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TelematicsDbContext>();
    await context.Database.EnsureCreatedAsync();
}

app.Run();
public partial class Program { }
