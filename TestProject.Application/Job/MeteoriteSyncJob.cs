using Quartz;
using TestProject.Application.Interfaces;
using TestProject.Core.Interfaces;

namespace TestProject.Application.Job
{
    [DisallowConcurrentExecution]
    public class MeteoriteSyncJob : IJob
    {
        private readonly IMeteoriteSyncService _meteoriteSyncService;
        private readonly ILogRepository _logger;

        public MeteoriteSyncJob(
            IMeteoriteSyncService meteoriteSyncService,
            ILogRepository logger)
        {
            _meteoriteSyncService = meteoriteSyncService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context) 
        {
            try
            {
                var result = await _meteoriteSyncService.SyncMeteoriteDataAsync(context.CancellationToken);

                if (result.IsSuccess)
                {
                   await _logger.LogInformationAsync(
                        $"Job completed successfully. {result.ProcessedCount} meteorites processed",
                         "MeteoriteSyncJob");
                }
                else
                {
                    await _logger.LogInformationAsync(
                        $"Job completed with errors: {result.Error.Message}",
                        "MeteoriteSyncJob");
                }
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync("Critical error in job", ex, "MeteoriteSyncJob");
                throw new JobExecutionException(ex, false);
            }
        }
    }
}
