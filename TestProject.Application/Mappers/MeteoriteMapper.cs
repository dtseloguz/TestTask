using TestProject.Application.DTO;
using TestProject.Core.Entities;
using TestProject.Core.UseCases;

namespace TestProject.Application.Mappers
{
    public static class MeteoriteMapper
    {
        public static Meteorite mapToMeteorite(MeteoriteDTO dto)
        {
            var result = new Meteorite()
            {
                Id = dto.Id,
                Name = dto.Name,
                NameType = dto.NameType,
                RecClass = dto.RecClass,
                Mass = dto.Mass,   
                Fall = dto.Fall,
                Year = dto.Year.Year,
                Reclat = dto.Reclat,
                Reclong = dto.Reclong
            };

            if (dto.GeoLocation is not null) 
            {
                result.Geolocation = dto.GeoLocation.ToPoint();
            }

            return result;
        }

        public static IEnumerable<Meteorite> mapToMeteorite(IEnumerable<MeteoriteDTO> list)
        {
            return list.Select(dto => mapToMeteorite(dto)).ToList();
        }

        public static MeteoritesByYearDTO mapToMeteoritesByYearDTO(MeteoritesByYear obj)
        {
            var result = new MeteoritesByYearDTO()
            {
                Year = obj.Year,
                Count = obj.Count,
                TotalMass = obj.TotalMass
            };

            return result;
        }

        public static List<MeteoritesByYearDTO> mapToMeteoritesByYearDTO(List<MeteoritesByYear> list)
        {
            return list.Select(dto => mapToMeteoritesByYearDTO(dto)).ToList();
        }
    }
}
