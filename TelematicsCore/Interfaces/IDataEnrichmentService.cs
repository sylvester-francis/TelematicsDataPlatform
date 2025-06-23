using TelematicsCore.Models;

namespace TelematicsCore.Interfaces
{
    public interface IDataEnrichmentService
    {
        Task<TelematicsEvent> EnrichEventDataAsync(TelematicsEvent telematicsEvent);
        Task<IEnumerable<Alert>> GenerateAlertsAsync(TelematicsEvent telematicsEvent);
        Task<Trip?> ProcessTripDataAsync(TelematicsEvent telematicsEvent);
    }
}