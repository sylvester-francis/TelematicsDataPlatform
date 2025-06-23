using System.ComponentModel.DataAnnotations;

namespace TelematicsCore.DTOs
{
    public class TelematicsEventDto
    {
        [Required]
        public string VehicleIdentifier { get; set; } = string.Empty;
        
        [Required]
        public DateTime Timestamp { get; set; }
        
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Speed { get; set; }
        public double? Heading { get; set; }
        public double? Altitude { get; set; }
        public double? Odometer { get; set; }
        public double? FuelLevel { get; set; }
        public double? EngineLoad { get; set; }
        public int? EngineRPM { get; set; }
        public double? EngineCoolantTemperature { get; set; }
        public string EventType { get; set; } = "POSITION";
        public string? AdditionalData { get; set; }
    }
    
    public class BatchTelematicsEventDto
    {
        [Required]
        public List<TelematicsEventDto> Events { get; set; } = new();
    }
    
    public class VehicleStatsDto
    {
        public string VehicleIdentifier { get; set; } = string.Empty;
        public int TotalEvents { get; set; }
        public DateTime? LastEventTime { get; set; }
        public double? LastKnownSpeed { get; set; }
        public double? LastKnownLatitude { get; set; }
        public double? LastKnownLongitude { get; set; }
        public int ActiveTrips { get; set; }
        public int TotalAlerts { get; set; }
    }
}