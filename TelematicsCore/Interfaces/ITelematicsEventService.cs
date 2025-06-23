using TelematicsCore.DTOs;
using TelematicsCore.Models;

namespace TelematicsCore.Interfaces
{
    public interface ITelematicsEventService
    {
        Task<TelematicsEvent> ProcessEventAsync(TelematicsEventDto eventDto);
        Task<IEnumerable<TelematicsEvent>> ProcessBatchEventsAsync(BatchTelematicsEventDto batchDto);
        Task<IEnumerable<TelematicsEvent>> GetVehicleEventsAsync(string vehicleIdentifier, DateTime? startTime = null, DateTime? endTime = null);
        Task<IEnumerable<TelematicsEvent>> GetUnprocessedEventsAsync(int batchSize = 1000);
        Task MarkEventsAsProcessedAsync(IEnumerable<long> eventIds);
    }
}