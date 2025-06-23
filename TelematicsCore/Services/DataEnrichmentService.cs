using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TelematicsCore.Interfaces;
using TelematicsCore.Models;

namespace TelematicsCore.Services
{
    public class DataEnrichmentService : IDataEnrichmentService
    {
        private readonly DbContext _context;
        private readonly ILogger<DataEnrichmentService> _logger;

        public DataEnrichmentService(DbContext context, ILogger<DataEnrichmentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<TelematicsEvent> EnrichEventDataAsync(TelematicsEvent telematicsEvent)
        {
            // Enrich with calculated fields, external data, etc.
            // This is where you'd integrate with external services for weather, traffic, etc.
            
            _logger.LogDebug("Enriching event {EventId} for vehicle {VehicleId}", 
                telematicsEvent.Id, telematicsEvent.VehicleId);

            return Task.FromResult(telematicsEvent);
        }

        public async Task<IEnumerable<Alert>> GenerateAlertsAsync(TelematicsEvent telematicsEvent)
        {
            var alerts = new List<Alert>();

            // Speed alerts
            if (telematicsEvent.Speed > 120) // Speed limit exceeded
            {
                alerts.Add(new Alert
                {
                    TelematicsEventId = telematicsEvent.Id,
                    VehicleId = telematicsEvent.VehicleId,
                    AlertType = "SPEEDING",
                    Severity = "WARNING",
                    Description = $"Vehicle exceeded speed limit: {telematicsEvent.Speed:F1} km/h",
                    CreatedAt = DateTime.UtcNow
                });
            }

            // Engine alerts
            if (telematicsEvent.EngineCoolantTemperature > 100)
            {
                alerts.Add(new Alert
                {
                    TelematicsEventId = telematicsEvent.Id,
                    VehicleId = telematicsEvent.VehicleId,
                    AlertType = "ENGINE_OVERHEATING",
                    Severity = "CRITICAL",
                    Description = $"Engine coolant temperature high: {telematicsEvent.EngineCoolantTemperature:F1}°C",
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (alerts.Any())
            {
                _context.Set<Alert>().AddRange(alerts);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Generated {AlertCount} alerts for event {EventId}", 
                    alerts.Count, telematicsEvent.Id);
            }

            return alerts;
        }

        public async Task<Trip?> ProcessTripDataAsync(TelematicsEvent telematicsEvent)
        {
            // Simplified trip detection logic
            // In production, this would be more sophisticated
            
            var lastEvent = await _context.Set<TelematicsEvent>()
                .Where(e => e.VehicleId == telematicsEvent.VehicleId && e.Id < telematicsEvent.Id)
                .OrderByDescending(e => e.Timestamp)
                .FirstOrDefaultAsync();

            if (lastEvent == null)
                return null;

            var timeDiff = telematicsEvent.Timestamp - lastEvent.Timestamp;
            
            // If more than 30 minutes since last event, consider this a new trip
            if (timeDiff.TotalMinutes > 30)
            {
                var trip = new Trip
                {
                    VehicleId = telematicsEvent.VehicleId,
                    StartTime = telematicsEvent.Timestamp,
                    StartLocation = telematicsEvent.Location,
                    EventCount = 1,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Set<Trip>().Add(trip);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Started new trip {TripId} for vehicle {VehicleId}", 
                    trip.Id, telematicsEvent.VehicleId);

                return trip;
            }

            return null;
        }
    }
}