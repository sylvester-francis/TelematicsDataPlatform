using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using TelematicsCore.DTOs;
using TelematicsCore.Interfaces;
using TelematicsCore.Models;

namespace TelematicsCore.Services
{
    public class TelematicsEventService : ITelematicsEventService
    {
        private readonly DbContext _context;
        private readonly IVehicleService _vehicleService;
        private readonly IDataEnrichmentService _enrichmentService;
        private readonly ILogger<TelematicsEventService> _logger;

        public TelematicsEventService(
            DbContext context,
            IVehicleService vehicleService,
            IDataEnrichmentService enrichmentService,
            ILogger<TelematicsEventService> logger)
        {
            _context = context;
            _vehicleService = vehicleService;
            _enrichmentService = enrichmentService;
            _logger = logger;
        }

        public async Task<TelematicsEvent> ProcessEventAsync(TelematicsEventDto eventDto)
        {
            // Get or create vehicle
            var vehicle = await _vehicleService.GetVehicleByIdentifierAsync(eventDto.VehicleIdentifier);
            if (vehicle == null)
            {
                vehicle = await _vehicleService.CreateVehicleAsync(new Vehicle
                {
                    VehicleIdentifier = eventDto.VehicleIdentifier
                });
            }

            // Create telematics event
            var telematicsEvent = new TelematicsEvent
            {
                VehicleId = vehicle.Id,
                Timestamp = eventDto.Timestamp,
                Speed = eventDto.Speed,
                Heading = eventDto.Heading,
                Altitude = eventDto.Altitude,
                Odometer = eventDto.Odometer,
                FuelLevel = eventDto.FuelLevel,
                EngineLoad = eventDto.EngineLoad,
                EngineRPM = eventDto.EngineRPM,
                EngineCoolantTemperature = eventDto.EngineCoolantTemperature,
                EventType = eventDto.EventType,
                AdditionalData = eventDto.AdditionalData,
                ProcessedAt = DateTime.UtcNow,
                IsProcessed = false
            };

            // Set location if coordinates are provided
            if (eventDto.Latitude.HasValue && eventDto.Longitude.HasValue)
            {
                var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
                telematicsEvent.Location = geometryFactory.CreatePoint(
                    new Coordinate(eventDto.Longitude.Value, eventDto.Latitude.Value));
            }

            _context.Set<TelematicsEvent>().Add(telematicsEvent);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Processed telematics event for vehicle {VehicleIdentifier} at {Timestamp}", 
                eventDto.VehicleIdentifier, eventDto.Timestamp);

            return telematicsEvent;
        }

        public async Task<IEnumerable<TelematicsEvent>> ProcessBatchEventsAsync(BatchTelematicsEventDto batchDto)
        {
            var results = new List<TelematicsEvent>();
            
            foreach (var eventDto in batchDto.Events)
            {
                try
                {
                    var result = await ProcessEventAsync(eventDto);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process event for vehicle {VehicleIdentifier}", 
                        eventDto.VehicleIdentifier);
                }
            }

            _logger.LogInformation("Processed {Count} out of {Total} events in batch", 
                results.Count, batchDto.Events.Count);

            return results;
        }

        public async Task<IEnumerable<TelematicsEvent>> GetVehicleEventsAsync(
            string vehicleIdentifier, DateTime? startTime = null, DateTime? endTime = null)
        {
            var query = _context.Set<TelematicsEvent>()
                .Include(e => e.Vehicle)
                .Where(e => e.Vehicle.VehicleIdentifier == vehicleIdentifier);

            if (startTime.HasValue)
                query = query.Where(e => e.Timestamp >= startTime.Value);

            if (endTime.HasValue)
                query = query.Where(e => e.Timestamp <= endTime.Value);

            return await query.OrderBy(e => e.Timestamp).ToListAsync();
        }

        public async Task<IEnumerable<TelematicsEvent>> GetUnprocessedEventsAsync(int batchSize = 1000)
        {
            return await _context.Set<TelematicsEvent>()
                .Where(e => !e.IsProcessed)
                .OrderBy(e => e.Timestamp)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task MarkEventsAsProcessedAsync(IEnumerable<long> eventIds)
        {
            var events = await _context.Set<TelematicsEvent>()
                .Where(e => eventIds.Contains(e.Id))
                .ToListAsync();

            foreach (var evt in events)
            {
                evt.IsProcessed = true;
                evt.ProcessedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}