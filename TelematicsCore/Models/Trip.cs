using System.Drawing;

namespace TelematicsCore.Models
{
    public class Trip
    {
        public long Id { get; set; }
        public int VehicleId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public Point? StartLocation { get; set; }
        public Point? EndLocation { get; set; }

        public double? DistanceTraveled { get; set; } // km
        public double? AverageSpeed { get; set; } // km/h
        public double? MaxSpeed { get; set; } // km/h
        public double? FuelConsumed { get; set; } // liters

        public int EventCount { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual Vehicle Vehicle { get; set; } = null!;
    }
}