using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TelematicsCore.DTOs;
using TelematicsCore.Interfaces;
using TelematicsCore.Models;

namespace TelematicsCore.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly DbContext _context;
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(DbContext context, ILogger<VehicleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Vehicle?> GetVehicleByIdentifierAsync(string vehicleIdentifier)
        {
            return await _context.Set<Vehicle>()
                .FirstOrDefaultAsync(v => v.VehicleIdentifier == vehicleIdentifier);
        }

        public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle)
        {
            vehicle.CreatedAt = DateTime.UtcNow;
            vehicle.UpdatedAt = DateTime.UtcNow;
            
            _context.Set<Vehicle>().Add(vehicle);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Created new vehicle with identifier {VehicleIdentifier}", vehicle.VehicleIdentifier);
            return vehicle;
        }

        public async Task<VehicleStatsDto> GetVehicleStatsAsync(string vehicleIdentifier)
        {
            var vehicle = await _context.Set<Vehicle>()
                .Include(v => v.TelematicsEvents)
                .Include(v => v.Trips)
                .FirstOrDefaultAsync(v => v.VehicleIdentifier == vehicleIdentifier);

            if (vehicle == null)
                throw new ArgumentException($"Vehicle with identifier {vehicleIdentifier} not found");

            var lastEvent = vehicle.TelematicsEvents
                .OrderByDescending(e => e.Timestamp)
                .FirstOrDefault();

            var totalAlerts = await _context.Set<Alert>()
                .CountAsync(a => a.VehicleId == vehicle.Id);

            return new VehicleStatsDto
            {
                VehicleIdentifier = vehicle.VehicleIdentifier,
                TotalEvents = vehicle.TelematicsEvents.Count,
                LastEventTime = lastEvent?.Timestamp,
                LastKnownSpeed = lastEvent?.Speed,
                LastKnownLatitude = lastEvent?.Location?.Y,
                LastKnownLongitude = lastEvent?.Location?.X,
                ActiveTrips = vehicle.Trips.Count(t => t.EndTime == null),
                TotalAlerts = totalAlerts
            };
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            return await _context.Set<Vehicle>().ToListAsync();
        }
    }
}