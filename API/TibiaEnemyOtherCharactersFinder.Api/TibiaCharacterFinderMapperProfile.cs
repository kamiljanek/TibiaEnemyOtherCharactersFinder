using AutoMapper;
using TibiaEnemyOtherCharactersFinder.Api.Dtos;
using TibiaEnemyOtherCharactersFinder.Api.Entities;
using TibiaEnemyOtherCharactersFinder.Api.Models;

namespace TibiaEnemyOtherCharactersFinder.Api
{
    public class TibiaCharacterFinderMapperProfile : Profile
    {

        public TibiaCharacterFinderMapperProfile()
        {
            CreateMap<Character, CharacterWithCorrelationsResult>();

            CreateMap<CharacterCorrelation, WorldCorrelationDto>();

            CreateMap<World, WorldDto>();

            CreateMap<WorldScan, WorldScanDto>()
            .ForMember(m => m.World, c => c.MapFrom(s => s.World.Name));

            CreateMap<CreateWorldDto, World>();


        }
    }
}
