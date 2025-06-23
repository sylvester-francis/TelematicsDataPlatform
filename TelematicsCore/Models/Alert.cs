namespace TelematicsCore.Models
{
    public class Alert
    {
        public long Id { get; set; }
        public long TelematicsEventId { get; set; }
        public int VehicleId { get; set; }
        
        public string AlertType { get; set; } = string.Empty; // SPEEDING, HARSH_BRAKING, etc.
        public string Severity { get; set; } = "INFO"; // INFO, WARNING, CRITICAL
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsAcknowledged { get; set; }
        
        // Navigation properties
        public virtual TelematicsEvent TelematicsEvent { get; set; } = null!;
        public virtual Vehicle Vehicle { get; set; } = null!;
    }
}