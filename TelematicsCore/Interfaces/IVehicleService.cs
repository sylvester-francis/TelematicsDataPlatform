using TelematicsCore.DTOs;
using TelematicsCore.Models;

namespace TelematicsCore.Interfaces
{
    public interface IVehicleService
    {
        Task<Vehicle?> GetVehicleByIdentifierAsync(string vehicleIdentifier);
        Task<Vehicle> CreateVehicleAsync(Vehicle vehicle);
        Task<VehicleStatsDto> GetVehicleStatsAsync(string vehicleIdentifier);
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();
    }
}