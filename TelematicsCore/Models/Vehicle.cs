using System.ComponentModel.DataAnnotations;

namespace TelematicsCore.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string VehicleIdentifier { get; set; } = string.Empty;

        [StringLength(100)]
        public string Make { get; set; } = string.Empty;

        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        public int Year { get; set; }

        [StringLength(17)]
        public string VIN { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<TelematicsEvent> TelematicsEvents { get; set; } = new List<TelematicsEvent>();
        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}