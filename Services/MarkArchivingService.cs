using ClassAPI.Data;
using ClassAPI.Models.Entities;

namespace ClassAPI.Services
{
    public class MarkArchivingService : BackgroundService
    {
        private readonly ILogger<MarkArchivingService> _logger;
        private readonly TimeSpan _archivingInterval = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _markAgeThreshold = TimeSpan.FromMinutes(8);

        public MarkArchivingService(ILogger<MarkArchivingService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Mark Archiving Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ArchiveOldMarks();
                    await Task.Delay(_archivingInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Mark Archiving Service is stopping");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while archiving marks");
                    // Wait a bit before retrying to avoid rapid error loops
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        private async Task ArchiveOldMarks()
        {
            var cutoffTime = DateTime.Now - _markAgeThreshold;
            var marksToArchive = InMemoryDatabase.Marks.Values
                .Where(mark => !mark.IsArchived && mark.AddedAt <= cutoffTime)
                .ToList();

            if (marksToArchive.Any())
            {
                _logger.LogInformation("Archiving {Count} marks older than {Threshold} minutes", 
                    marksToArchive.Count, _markAgeThreshold.TotalMinutes);

                foreach (var mark in marksToArchive)
                {
                    mark.IsArchived = true;
                    _logger.LogDebug("Archived mark {MarkId} created at {CreatedAt}", 
                        mark.Id, mark.AddedAt);
                }

                _logger.LogInformation("Successfully archived {Count} marks", marksToArchive.Count);
            }
            else
            {
                _logger.LogDebug("No marks found to archive");
            }

            await Task.CompletedTask;
        }
    }
}
