using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;
using System.Data;
using TestProject.Common.Enums;
using TestProject.Common.Models;
using TestProject.Core.Entities;
using TestProject.Core.Interfaces;
using TestProject.Core.UseCases;

namespace TestProject.Infrastructure.Repositories
{
    public class MeteoriteRepository : IMeteoriteRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogRepository _logger;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public MeteoriteRepository(
            AppDbContext dbContext,
            ILogRepository logger,
            IMemoryCache cache)
        {
            _dbContext = dbContext;
            _logger = logger;
            _cache = cache;
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
        }

        public async Task<Result<List<int>>> GetYearsList(CancellationToken cancellationToken)
        {
            const string cacheKey = "MeteoriteYearsList";

            if (_cache.TryGetValue(cacheKey, out List<int> cachedYears))
            {
                return Result<List<int>>.Success(cachedYears ?? []);
            }

            var years = await _dbContext.Meteorite
                .Select(x => x.Year)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync(cancellationToken);

            _cache.Set(cacheKey, years, _cacheOptions);

            return Result<List<int>>.Success(years);
        }

        public async Task<Result<List<string>>> GetRecClassList(CancellationToken cancellationToken)
        {
            const string cacheKey = "MeteoriteRecClassList";

            if (_cache.TryGetValue(cacheKey, out List<string> cachedClasses))
            {
                return Result<List<string>>.Success(cachedClasses ?? []);
            }

            var classes = await _dbContext.Meteorite
                .Select(x => x.RecClass)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync(cancellationToken);

            _cache.Set(cacheKey, classes, _cacheOptions);

            return Result<List<string>>.Success(classes);
        }

        public async Task<Result<List<MeteoritesByYear>>> SearchMeteorites(
            int? minYear,
            int? maxYear,
            string? recClass,
            string? name,
            MeteoriteByYearSort sort,
            CancellationToken cancellationToken)
        {
            if (minYear > maxYear)
            {
                return Result<List<MeteoritesByYear>>.Failure(Error.HandleError("Минимальный год больше максимального."));
            }

            if (minYear < 0 || maxYear < 0)
            {
                return Result<List<MeteoritesByYear>>.Failure(Error.HandleError("Год не может быть меньше нуля."));
            }

            try
            {
                var query = _dbContext.Meteorite.AsQueryable();

                if (minYear.HasValue)
                    query = query.Where(m => m.Year >= minYear.Value);

                if (maxYear.HasValue)
                    query = query.Where(m => m.Year <= maxYear.Value);

                if (!string.IsNullOrEmpty(recClass))
                    query = query.Where(m => m.RecClass == recClass);

                if (!string.IsNullOrEmpty(name))
                    query = query.Where(m => EF.Functions.Like(m.Name, $"%{name}%"));

                var groupedQuery = query
                    .GroupBy(m => m.Year)
                    .Select(g => new MeteoritesByYear
                    {
                        Year = g.Key,
                        Count = g.Count(),
                        TotalMass = g.Sum(m => m.Mass)
                    });

                groupedQuery = ApplySorting(groupedQuery, sort);

                var result = await groupedQuery.ToListAsync(cancellationToken);
                return Result<List<MeteoritesByYear>>.Success(result);
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync("Meteorite search failed", ex, "MeteoriteRepository");
                return Result<List<MeteoritesByYear>>.Failure(Error.DatabaseFailure());
            }
        }

        public async Task<Result<int>> UpsertMeteoritesAsync(
            IEnumerable<Meteorite> meteorites,
            CancellationToken cancellationToken)
        {
            var connection = (NpgsqlConnection)_dbContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync(cancellationToken);

                await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
                var processedCount = 0;

                try
                {
                    await CreateTempTable(connection, transaction);
                    processedCount = await BulkInsertData(connection, meteorites, cancellationToken);

                    await SyncData(connection, cancellationToken);

                    await transaction.CommitAsync(cancellationToken);
                    return Result<int>.Success(processedCount);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync("Meteorite upsert failed", ex, "MeteoriteRepository");
                return Result<int>.Failure(Error.DatabaseFailure());
            }
        }

        #region Private Methods

        private IQueryable<MeteoritesByYear> ApplySorting(
            IQueryable<MeteoritesByYear> query,
            MeteoriteByYearSort sort)
        {
            return sort?.Property switch
            {
                MeteoritesByYearSortParam.Year => sort.Type == SortType.Asc
                    ? query.OrderBy(x => x.Year)
                    : query.OrderByDescending(x => x.Year),

                MeteoritesByYearSortParam.Count => sort.Type == SortType.Asc
                    ? query.OrderBy(x => x.Count)
                    : query.OrderByDescending(x => x.Count),

                MeteoritesByYearSortParam.TotalMass => sort.Type == SortType.Asc
                    ? query.OrderBy(x => x.TotalMass)
                    : query.OrderByDescending(x => x.TotalMass),

                _ => query.OrderByDescending(x => x.Year)
            };
        }

        private async Task CreateTempTable(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            await using var cmd = new NpgsqlCommand(@"
                CREATE TEMPORARY TABLE TempMeteoriteData (
                    Id integer PRIMARY KEY,
                    Name TEXT,
                    NameType TEXT,
                    RecClass TEXT,
                    Mass NUMERIC,
                    Fall TEXT,
                    Year integer,
                    Reclat numeric(9, 6),
                    Reclong numeric(9,6),
                    Geolocation geometry(Point,4326)
                ) ON COMMIT DROP;", connection, transaction);

            await cmd.ExecuteNonQueryAsync();
        }

        private async Task<int> BulkInsertData(
            NpgsqlConnection connection,
            IEnumerable<Meteorite> meteorites,
            CancellationToken cancellationToken)
        {
            var count = 0;

            await using var writer = await connection.BeginBinaryImportAsync(
                "COPY TempMeteoriteData (Id, Name, NameType, RecClass, Mass, Fall, Year, Reclat, Reclong, Geolocation) " +
                "FROM STDIN (FORMAT BINARY)", cancellationToken);

            foreach (var item in meteorites)
            {
                await writer.StartRowAsync(cancellationToken);
                await writer.WriteAsync(item.Id, cancellationToken);
                await writer.WriteAsync(item.Name ?? (object)DBNull.Value, cancellationToken);
                await writer.WriteAsync(item.NameType ?? (object)DBNull.Value, cancellationToken);
                await writer.WriteAsync(item.RecClass ?? (object)DBNull.Value, cancellationToken);
                await writer.WriteAsync(item.Mass ?? (object)DBNull.Value, cancellationToken);
                await writer.WriteAsync(item.Fall ?? (object)DBNull.Value, cancellationToken);
                await writer.WriteAsync(item.Year, cancellationToken);
                await writer.WriteAsync(item.Reclat, cancellationToken);
                await writer.WriteAsync(item.Reclong, cancellationToken);
                await writer.WriteAsync(item.Geolocation, cancellationToken);
                count++;
            }

            await writer.CompleteAsync(cancellationToken);
            return count;
        }

        private async Task SyncData(NpgsqlConnection connection, CancellationToken cancellationToken)
        {
            await _dbContext.Database.ExecuteSqlRawAsync(@"
                DELETE FROM ""Meteorite""
                WHERE ""Id"" NOT IN (SELECT Id FROM TempMeteoriteData)", cancellationToken);

            await _dbContext.Database.ExecuteSqlRawAsync(@"
                UPDATE ""Meteorite""
                SET 
                    ""Name"" = temp.Name,
                    ""NameType"" = temp.NameType,
                    ""RecClass"" = temp.RecClass,
                    ""Mass"" = temp.Mass,
                    ""Fall"" = temp.Fall,
                    ""Year"" = temp.Year,
                    ""Reclat"" = temp.Reclat,
                    ""Reclong"" = temp.Reclong,
                    ""Geolocation"" = temp.Geolocation
                FROM TempMeteoriteData temp
                WHERE ""Meteorite"".""Id"" = temp.Id", cancellationToken);

            await _dbContext.Database.ExecuteSqlRawAsync(@"
                INSERT INTO ""Meteorite"" 
                (""Id"", ""Name"", ""NameType"", ""RecClass"", ""Mass"", ""Fall"", ""Year"", ""Reclat"", ""Reclong"", ""Geolocation"")
                SELECT 
                    temp.Id, temp.Name, temp.NameType, temp.RecClass, 
                    temp.Mass, temp.Fall, temp.Year, temp.Reclat, 
                    temp.Reclong, temp.Geolocation
                FROM TempMeteoriteData temp
                LEFT JOIN ""Meteorite"" m ON m.""Id"" = temp.Id
                WHERE m.""Id"" IS NULL", cancellationToken);
        }

        #endregion
    }
}