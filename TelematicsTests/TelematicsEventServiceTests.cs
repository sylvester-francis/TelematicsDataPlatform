using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TelematicsCore.DTOs;
using TelematicsCore.Services;
using TelematicsData;
using Xunit;

namespace TelematicsTests
{
    public class TelematicsEventServiceTests : IDisposable
    {
        private readonly TelematicsDbContext _context;
        private readonly TelematicsEventService _eventService;
        private readonly VehicleService _vehicleService;
        private readonly DataEnrichmentService _enrichmentService;

        public TelematicsEventServiceTests()
        {
            var options = new DbContextOptionsBuilder<TelematicsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TelematicsDbContext(options);
            
            var loggerFactory = new LoggerFactory();
            var vehicleLogger = loggerFactory.CreateLogger<VehicleService>();
            var eventLogger = loggerFactory.CreateLogger<TelematicsEventService>();
            var enrichmentLogger = loggerFactory.CreateLogger<DataEnrichmentService>();
            
            _vehicleService = new VehicleService(_context, vehicleLogger);
            _enrichmentService = new DataEnrichmentService(_context, enrichmentLogger);
            _eventService = new TelematicsEventService(_context, _vehicleService, _enrichmentService, eventLogger);
        }

        [Fact]
        public async Task ProcessEventAsync_ShouldCreateEvent()
        {
            // Arrange
            var eventDto = new TelematicsEventDto
            {
                VehicleIdentifier = "TEST-VEHICLE-001",
                Timestamp = DateTime.UtcNow,
                Latitude = 43.4643,
                Longitude = -80.5204,
                Speed = 50.5,
                EventType = "POSITION"
            };

            // Act
            var result = await _eventService.ProcessEventAsync(eventDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(eventDto.VehicleIdentifier, result.Vehicle.VehicleIdentifier);
            Assert.Equal(eventDto.Speed, result.Speed);
            Assert.NotNull(result.Location);
        }

        [Fact]
        public async Task ProcessBatchEventsAsync_ShouldProcessMultipleEvents()
        {
            // Arrange
            var batchDto = new BatchTelematicsEventDto
            {
                Events = new List<TelematicsEventDto>
                {
                    new()
                    {
                        VehicleIdentifier = "BATCH-001",
                        Timestamp = DateTime.UtcNow.AddMinutes(-2),
                        Speed = 40.0,
                        EventType = "POSITION"
                    },
                    new()
                    {
                        VehicleIdentifier = "BATCH-001",
                        Timestamp = DateTime.UtcNow.AddMinutes(-1),
                        Speed = 45.0,
                        EventType = "POSITION"
                    }
                }
            };

            // Act
            var results = await _eventService.ProcessBatchEventsAsync(batchDto);

            // Assert
            Assert.Equal(2, results.Count());
            Assert.All(results, r => Assert.Equal("BATCH-001", r.Vehicle.VehicleIdentifier));
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}