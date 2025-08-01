using MeteoriteSync.Application.Services.Results;
using TestProject.Application.Interfaces;
using TestProject.Core.Interfaces;

namespace TestProject.Application.Services
{
    public class MeteoriteSyncService : IMeteoriteSyncService
    {
        private readonly IMeteoritesExternalDataService _meteoritesExternalDataService;
        private readonly IMeteoriteRepository _meteoriteRepository;
        private readonly ILogRepository _logger;

        public MeteoriteSyncService(
            IMeteoritesExternalDataService meteoritesExternalDataService, IMeteoriteRepository meteoriteRepository, ILogRepository logger)
 
        {
            _meteoritesExternalDataService = meteoritesExternalDataService;
            _meteoriteRepository = meteoriteRepository;
            _logger = logger;
        }

        public async Task<SyncResult> SyncMeteoriteDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _logger.LogInformationAsync("Starting meteorite data synchronization", "MeteoriteSyncService");

                var meteorites = await _meteoritesExternalDataService.GetMeteoriteDataAsync(cancellationToken);

                var result = await _meteoriteRepository.UpsertMeteoritesAsync(meteorites, cancellationToken);

                if (!result.IsSuccess)
                {
                    new SyncResult(false, 0, result.Error);
                }

               await _logger.LogInformationAsync($"Synchronization completed successfully. {result.Value} meteorites processed", "MeteoriteSyncService");

                return new SyncResult(true, result.Value, Error.None);
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync("Error during meteorite data synchronization", ex, "MeteoriteSyncService");
                return new SyncResult(false, 0, Error.ServerError());
            }
        }
    }
}
