using TestProject.Common.Models;
using TestProject.Core.Entities;
using TestProject.Core.UseCases;

namespace TestProject.Core.Interfaces
{
    public interface IMeteoriteRepository
    {
        public Task<Result<int>> UpsertMeteoritesAsync(IEnumerable<Meteorite> meteorites, CancellationToken cancellationToken);
        public Task<Result<List<MeteoritesByYear>>> SearchMeteorites(int? minYear, int? maxYear, string? recClass, string? name, MeteoriteByYearSort sort, CancellationToken cancellationToken);
        public Task<Result<List<int>>> GetYearsList(CancellationToken cancellationToken);
        public Task<Result<List<string>>> GetRecClassList(CancellationToken cancellationToken);
    }
}
