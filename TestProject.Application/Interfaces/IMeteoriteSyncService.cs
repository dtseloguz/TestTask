using MeteoriteSync.Application.Services.Results;

namespace TestProject.Application.Interfaces
{
    public interface IMeteoriteSyncService
    {
        public Task<SyncResult> SyncMeteoriteDataAsync(CancellationToken cancellationToken);
    }
}
