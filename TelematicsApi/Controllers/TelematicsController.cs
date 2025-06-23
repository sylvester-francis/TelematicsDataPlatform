using Microsoft.AspNetCore.Mvc;
using TelematicsCore.DTOs;
using TelematicsCore.Interfaces;

namespace TelematicsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelematicsController : ControllerBase
    {
        private readonly ITelematicsEventService _eventService;
        private readonly ILogger<TelematicsController> _logger;

        public TelematicsController(ITelematicsEventService eventService, ILogger<TelematicsController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        [HttpPost("events")]
        public async Task<ActionResult> SubmitEvent([FromBody] TelematicsEventDto eventDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _eventService.ProcessEventAsync(eventDto);
                return Ok(new { EventId = result.Id, Status = "Processed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing telematics event");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("events/batch")]
        public async Task<ActionResult> SubmitBatchEvents([FromBody] BatchTelematicsEventDto batchDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var results = await _eventService.ProcessBatchEventsAsync(batchDto);
                return Ok(new { 
                    ProcessedCount = results.Count(), 
                    TotalSubmitted = batchDto.Events.Count,
                    Status = "Batch processed" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing batch telematics events");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("vehicles/{vehicleIdentifier}/events")]
        public async Task<ActionResult> GetVehicleEvents(
            string vehicleIdentifier,
            [FromQuery] DateTime? startTime = null,
            [FromQuery] DateTime? endTime = null)
        {
            try
            {
                var events = await _eventService.GetVehicleEventsAsync(vehicleIdentifier, startTime, endTime);
                return Ok(events.Select(e => new
                {
                    e.Id,
                    e.Timestamp,
                    Latitude = e.Location?.Y,
                    Longitude = e.Location?.X,
                    e.Speed,
                    e.Heading,
                    e.EventType,
                    e.IsProcessed
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving events for vehicle {VehicleIdentifier}", vehicleIdentifier);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}