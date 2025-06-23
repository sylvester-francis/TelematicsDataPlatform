using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using TelematicsCore.DTOs;
using TelematicsData;
using Xunit;

namespace TelematicsTests
{
    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the real database
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TelematicsDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Add in-memory database for testing
                    services.AddDbContext<TelematicsDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });
                });
            });
            
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Health_Endpoint_ReturnsHealthy()
        {
            // Act
            var response = await _client.GetAsync("/api/health");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Healthy", content);
        }

        [Fact]
        public async Task SubmitEvent_ValidData_ReturnsSuccess()
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
            var response = await _client.PostAsJsonAsync("/api/telematics/events", eventDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.Equal("Processed", result.GetProperty("Status").GetString());
        }

        [Fact]
        public async Task SubmitBatchEvents_ValidData_ReturnsSuccess()
        {
            // Arrange
            var batchDto = new BatchTelematicsEventDto
            {
                Events = new List<TelematicsEventDto>
                {
                    new()
                    {
                        VehicleIdentifier = "BATCH-TEST-001",
                        Timestamp = DateTime.UtcNow.AddMinutes(-1),
                        Speed = 60.0,
                        EventType = "POSITION"
                    },
                    new()
                    {
                        VehicleIdentifier = "BATCH-TEST-001",
                        Timestamp = DateTime.UtcNow,
                        Speed = 65.0,
                        EventType = "POSITION"
                    }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/telematics/events/batch", batchDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.Equal(2, result.GetProperty("ProcessedCount").GetInt32());
        }

        [Fact]
        public async Task GetVehicles_ReturnsVehicleList()
        {
            // First, add a vehicle by submitting an event
            var eventDto = new TelematicsEventDto
            {
                VehicleIdentifier = "LIST-TEST-001",
                Timestamp = DateTime.UtcNow,
                Speed = 55.0,
                EventType = "POSITION"
            };
            await _client.PostAsJsonAsync("/api/telematics/events", eventDto);

            // Act
            var response = await _client.GetAsync("/api/vehicles");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("LIST-TEST-001", content);
        }

        [Fact]
        public async Task GetMetrics_ReturnsSystemMetrics()
        {
            // Act
            var response = await _client.GetAsync("/api/health/metrics");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            
            Assert.True(result.TryGetProperty("TotalVehicles", out _));
            Assert.True(result.TryGetProperty("TotalEvents", out _));
            Assert.True(result.TryGetProperty("UnprocessedEvents", out _));
            Assert.True(result.TryGetProperty("TotalAlerts", out _));
        }
    }
}