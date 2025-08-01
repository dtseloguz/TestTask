using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace TestProject.Application.DTO
{
    public class GeolocationDTO
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("coordinates")]
        public double[] Coordinates { get; set; }

        public Point ToPoint()
        {
            if (Coordinates == null || Coordinates.Length < 2)
            {
                throw new ArgumentException("Coordinates array must contain at least 2 elements (longitude and latitude)");
            }

            return new Point(Coordinates[0], Coordinates[1]);
        }
    }
}
