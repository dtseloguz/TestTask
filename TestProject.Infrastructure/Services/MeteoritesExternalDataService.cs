using System.Text.Json.Serialization;
using System.Text.Json;
using TestProject.Core.Interfaces;
using TestProject.Application.DTO;
using TestProject.Core.Entities;
using TestProject.Application.Mappers;

namespace TestProject.Infrastructure.Services
{
    public class MeteoritesExternalDataService : IMeteoritesExternalDataService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogRepository _logger;

        public MeteoritesExternalDataService(HttpClient httpClient, ILogRepository logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<Meteorite>> GetMeteoriteDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    "https://raw.githubusercontent.com/biggiko/nasa-dataset/main/y77d-th95.json",
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var dto= JsonSerializer.Deserialize<List<MeteoriteDTO>>(content, options)
                    ?? Enumerable.Empty<MeteoriteDTO>();

                if (dto is null) 
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                return MeteoriteMapper.mapToMeteorite(dto);
            }
            catch (HttpRequestException ex)
            {
                await _logger.LogErrorAsync("Error fetching data from NASA dataset", ex, "MeteoritesExternalDataService");
                throw;
            }
            catch (JsonException ex)
            {
                await _logger.LogErrorAsync("Error parsing NASA dataset JSON", ex, "MeteoritesExternalDataService");
                throw;
            }
        }
    }
}
