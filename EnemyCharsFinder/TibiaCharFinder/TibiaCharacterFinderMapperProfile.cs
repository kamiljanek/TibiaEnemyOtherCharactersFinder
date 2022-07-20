using AutoMapper;
using TibiaCharFinderAPI.Entities;
using TibiaCharFinderAPI.Models;

namespace TibiaCharFinderAPI
{
    public class TibiaCharacterFinderMapperProfile : Profile
    {

        public TibiaCharacterFinderMapperProfile()
        {
            CreateMap<Character, CharacterDto>();

            CreateMap<WorldCorrelation, WorldCorrelationDto>();

            CreateMap<World, WorldDto>();

            CreateMap<WorldScan, WorldScanDto>()
            .ForMember(m => m.World, c => c.MapFrom(s => s.World.Name));

            CreateMap<CreateWorldDto, World>();


        }
    }
}
