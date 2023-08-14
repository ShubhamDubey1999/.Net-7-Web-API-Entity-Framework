using DOTNET_RPG.Dtos.Fight;
using DOTNET_RPG.Dtos.Skill;
using DOTNET_RPG.Dtos.Weapon;

namespace DOTNET_RPG
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Characters, GetCharacterRequestDto>();
            CreateMap<AddCharacterResponseDto, Characters>();
            CreateMap<UpdateCharacterDto, Characters>();
            CreateMap<Weapon, GetWeaponDto>();
            CreateMap<Skill, GetSkillDto>();
            CreateMap<Characters , HighScoreDto>
        }
    }
}
