using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TelematicsCore.Interfaces;

namespace TelematicsBatchProcessor.Services
{
    public class BatchProcessingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BatchProcessingService> _logger;
        private readonly TimeSpan _processingInterval = TimeSpan.FromMinutes(5);

        public BatchProcessingService(IServiceProvider serviceProvider, ILogger<BatchProcessingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Batch Processing Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessUnprocessedEvents();
                    await Task.Delay(_processingInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in batch processing service");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        private async Task ProcessUnprocessedEvents()
        {
            using var scope = _serviceProvider.CreateScope();
            var eventService = scope.ServiceProvider.GetRequiredService<ITelematicsEventService>();
            var enrichmentService = scope.ServiceProvider.GetRequiredService<IDataEnrichmentService>();

            var unprocessedEvents = await eventService.GetUnprocessedEventsAsync(100);
            
            if (!unprocessedEvents.Any())
                return;

            _logger.LogInformation("Processing {Count} unprocessed events", unprocessedEvents.Count());

            var processedEventIds = new List<long>();

            foreach (var telematicsEvent in unprocessedEvents)
            {
                try
                {
                    // Enrich the event data
                    await enrichmentService.EnrichEventDataAsync(telematicsEvent);
                    
                    // Generate alerts
                    await enrichmentService.GenerateAlertsAsync(telematicsEvent);
                    
                    // Process trip data
                    await enrichmentService.ProcessTripDataAsync(telematicsEvent);

                    processedEventIds.Add(telematicsEvent.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing event {EventId}", telematicsEvent.Id);
                }
            }

            // Mark events as processed
            if (processedEventIds.Any())
            {
                await eventService.MarkEventsAsProcessedAsync(processedEventIds);
                _logger.LogInformation("Marked {Count} events as processed", processedEventIds.Count);
            }
        }
    }
}