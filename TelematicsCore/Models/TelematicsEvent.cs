using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace TelematicsCore.Models
{
    public class TelematicsEvent
    {
        public long Id { get; set; }

        public int VehicleId { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        // Geospatial data using NetTopologySuite
        public Point? Location { get; set; }

        public double? Speed { get; set; } // km/h
        public double? Heading { get; set; } // degrees
        public double? Altitude { get; set; } // meters
        public double? Odometer { get; set; } // km
        public double? FuelLevel { get; set; } // percentage
        public double? EngineLoad { get; set; } // percentage
        public int? EngineRPM { get; set; }
        public double? EngineCoolantTemperature { get; set; } // Celsius

        [StringLength(50)]
        public string EventType { get; set; } = "POSITION";

        [StringLength(500)]
        public string? AdditionalData { get; set; } // JSON string for extensibility

        public DateTime ProcessedAt { get; set; }
        public bool IsProcessed { get; set; }

        // Navigation properties
        public virtual Vehicle Vehicle { get; set; } = null!;
        public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();
    }
}