using DOTNET_RPG.Dtos.Weapon;

namespace DOTNET_RPG.Services.WeaponService
{
    public interface IWeaponService
    {
        Task<ServiceResponse<GetCharacterRequestDto>> AddWeapon(AddWeaponDto addWeapon);
    }
}
