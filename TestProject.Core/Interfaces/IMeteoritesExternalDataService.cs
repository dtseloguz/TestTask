using TestProject.Core.Entities;

namespace TestProject.Core.Interfaces
{
    public interface IMeteoritesExternalDataService
    {
        public Task<IEnumerable<Meteorite>> GetMeteoriteDataAsync(CancellationToken cancellationToken);
    }
}
