using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using TelematicsCore.Models;
using TelematicsCore.Services;
using TelematicsData;
using Xunit;

namespace TelematicsTests
{
    public class DataEnrichmentServiceTests : IDisposable
    {
        private readonly TelematicsDbContext _context;
        private readonly DataEnrichmentService _enrichmentService;

        public DataEnrichmentServiceTests()
        {
            var options = new DbContextOptionsBuilder<TelematicsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TelematicsDbContext(options);
            var logger = new Logger<DataEnrichmentService>(new LoggerFactory());
            _enrichmentService = new DataEnrichmentService(_context, logger);
        }

        [Fact]
        public async Task GenerateAlertsAsync_SpeedingEvent_ShouldCreateSpeedingAlert()
        {
            // Arrange
            var vehicle = new Vehicle { VehicleIdentifier = "ALERT-TEST-001", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            var telematicsEvent = new TelematicsEvent
            {
                VehicleId = vehicle.Id,
                Timestamp = DateTime.UtcNow,
                Speed = 150.0, // Exceeds speed limit
                EventType = "POSITION",
                ProcessedAt = DateTime.UtcNow
            };
            _context.TelematicsEvents.Add(telematicsEvent);
            await _context.SaveChangesAsync();

            // Act
            var alerts = await _enrichmentService.GenerateAlertsAsync(telematicsEvent);

            // Assert
            Assert.Single(alerts);
            var alert = alerts.First();
            Assert.Equal("SPEEDING", alert.AlertType);
            Assert.Equal("WARNING", alert.Severity);
            Assert.Contains("150", alert.Description);
        }

        [Fact]
        public async Task GenerateAlertsAsync_EngineOverheating_ShouldCreateCriticalAlert()
        {
            // Arrange
            var vehicle = new Vehicle { VehicleIdentifier = "TEMP-TEST-001", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            var telematicsEvent = new TelematicsEvent
            {
                VehicleId = vehicle.Id,
                Timestamp = DateTime.UtcNow,
                EngineCoolantTemperature = 110.0, // Overheating
                EventType = "ENGINE_DATA",
                ProcessedAt = DateTime.UtcNow
            };
            _context.TelematicsEvents.Add(telematicsEvent);
            await _context.SaveChangesAsync();

            // Act
            var alerts = await _enrichmentService.GenerateAlertsAsync(telematicsEvent);

            // Assert
            Assert.Single(alerts);
            var alert = alerts.First();
            Assert.Equal("ENGINE_OVERHEATING", alert.AlertType);
            Assert.Equal("CRITICAL", alert.Severity);
        }

        [Fact]
        public async Task ProcessTripDataAsync_NewTripCondition_ShouldCreateTrip()
        {
            // Arrange
            var vehicle = new Vehicle { VehicleIdentifier = "TRIP-TEST-001", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
            var location = geometryFactory.CreatePoint(new Coordinate(-80.5204, 43.4643));

            var telematicsEvent = new TelematicsEvent
            {
                VehicleId = vehicle.Id,
                Timestamp = DateTime.UtcNow,
                Location = location,
                EventType = "POSITION",
                ProcessedAt = DateTime.UtcNow
            };
            _context.TelematicsEvents.Add(telematicsEvent);
            await _context.SaveChangesAsync();

            // Act
            var trip = await _enrichmentService.ProcessTripDataAsync(telematicsEvent);

            // Assert - Since there's no previous event, should return null (no new trip created)
            Assert.Null(trip);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}