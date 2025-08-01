using System.Text.Json.Serialization;

namespace TestProject.Application.DTO
{
    public class MeteoriteDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("nametype")]
        public string NameType { get; set; }
        [JsonPropertyName("recclass")]
        public string RecClass { get; set; }
        [JsonPropertyName("mass")]
        public decimal? Mass { get; set; }
        [JsonPropertyName("fall")]
        public string Fall { get; set; }
        [JsonPropertyName("year")]
        public DateTime Year { get; set; }

        [JsonPropertyName("reclat")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal Reclat { get; set; }
        [JsonPropertyName("reclong")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal Reclong { get; set; }

        [JsonPropertyName("geolocation")]
        public GeolocationDTO? GeoLocation { get; set; }
    }
}
