using DOTNET_RPG.Dtos.Skill;
using DOTNET_RPG.Dtos.Weapon;

namespace DOTNET_RPG.Dtos.Character
{
    public class GetCharacterRequestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Frodo";
        public int Hitpoints { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defense { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public RPGClass Class { get; set; } = RPGClass.Knight;
        public GetWeaponDto? WeaponDto { get; set; }
        public List<GetSkillDto>? Skills { get; set; }
        public int Fights { get; set; }
        public int Victories { get; set; }
        public int Defeats { get; set; }
    }
}
