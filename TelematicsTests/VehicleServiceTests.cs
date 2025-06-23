using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TelematicsCore.Models;
using TelematicsCore.Services;
using TelematicsData;
using Xunit;

namespace TelematicsTests
{
    public class VehicleServiceTests : IDisposable
    {
        private readonly TelematicsDbContext _context;
        private readonly VehicleService _vehicleService;

        public VehicleServiceTests()
        {
            var options = new DbContextOptionsBuilder<TelematicsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TelematicsDbContext(options);
            var logger = new Logger<VehicleService>(new LoggerFactory());
            _vehicleService = new VehicleService(_context, logger);
        }

        [Fact]
        public async Task CreateVehicleAsync_ShouldCreateVehicle()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                VehicleIdentifier = "TEST-001",
                Make = "Toyota",
                Model = "Camry",
                Year = 2023
            };

            // Act
            var result = await _vehicleService.CreateVehicleAsync(vehicle);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TEST-001", result.VehicleIdentifier);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task GetVehicleByIdentifierAsync_ShouldReturnVehicle()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                VehicleIdentifier = "TEST-002",
                Make = "Honda",
                Model = "Civic"
            };
            await _vehicleService.CreateVehicleAsync(vehicle);

            // Act
            var result = await _vehicleService.GetVehicleByIdentifierAsync("TEST-002");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TEST-002", result.VehicleIdentifier);
            Assert.Equal("Honda", result.Make);
        }

        [Fact]
        public async Task GetVehicleByIdentifierAsync_NonExistentVehicle_ShouldReturnNull()
        {
            // Act
            var result = await _vehicleService.GetVehicleByIdentifierAsync("NON-EXISTENT");

            // Assert
            Assert.Null(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}