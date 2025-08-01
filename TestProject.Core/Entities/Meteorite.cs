using NetTopologySuite.Geometries;

namespace TestProject.Core.Entities
{
    public class Meteorite
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameType { get; set; }
        public string RecClass { get; set; }
        public decimal? Mass { get; set; }
        public string Fall { get; set; }
        public int Year { get; set; }

        public decimal Reclat { get; set; }
        public decimal Reclong { get; set; }

        public Point? Geolocation { get; set; }

        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }
    }
}
