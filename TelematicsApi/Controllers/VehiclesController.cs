using Microsoft.AspNetCore.Mvc;
using TelematicsCore.Interfaces;
using TelematicsCore.DTOs;

namespace TelematicsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(IVehicleService vehicleService, ILogger<VehiclesController> logger)
        {
            _vehicleService = vehicleService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllVehicles()
        {
            try
            {
                var vehicles = await _vehicleService.GetAllVehiclesAsync();
                return Ok(vehicles.Select(v => new
                {
                    v.Id,
                    v.VehicleIdentifier,
                    v.Make,
                    v.Model,
                    v.Year,
                    v.CreatedAt
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vehicles");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{vehicleIdentifier}/stats")]
        public async Task<ActionResult<VehicleStatsDto>> GetVehicleStats(string vehicleIdentifier)
        {
            try
            {
                var stats = await _vehicleService.GetVehicleStatsAsync(vehicleIdentifier);
                return Ok(stats);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vehicle stats for {VehicleIdentifier}", vehicleIdentifier);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}