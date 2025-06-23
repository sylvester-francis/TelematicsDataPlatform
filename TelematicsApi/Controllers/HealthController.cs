using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelematicsData;

namespace TelematicsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly TelematicsDbContext _context;
        private readonly ILogger<HealthController> _logger;

        public HealthController(TelematicsDbContext context, ILogger<HealthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetHealth()
        {
            try
            {
                // Check database connectivity
                var canConnect = await _context.Database.CanConnectAsync();
                
                var healthStatus = new
                {
                    Status = canConnect ? "Healthy" : "Unhealthy",
                    Timestamp = DateTime.UtcNow,
                    Database = canConnect ? "Connected" : "Disconnected",
                    Version = "1.0.0"
                };

                return canConnect ? Ok(healthStatus) : StatusCode(503, healthStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(503, new
                {
                    Status = "Unhealthy",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
            }
        }

        [HttpGet("metrics")]
        public async Task<ActionResult> GetMetrics()
        {
            try
            {
                var vehicleCount = await _context.Vehicles.CountAsync();
                var eventCount = await _context.TelematicsEvents.CountAsync();
                var unprocessedEvents = await _context.TelematicsEvents.CountAsync(e => !e.IsProcessed);
                var alertCount = await _context.Alerts.CountAsync();

                return Ok(new
                {
                    TotalVehicles = vehicleCount,
                    TotalEvents = eventCount,
                    UnprocessedEvents = unprocessedEvents,
                    TotalAlerts = alertCount,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving metrics");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}