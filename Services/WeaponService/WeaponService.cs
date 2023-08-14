using DOTNET_RPG.Dtos.Weapon;
using System.Security.Claims;

namespace DOTNET_RPG.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly DataContext _db;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public WeaponService(DataContext dataContext , IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _db = dataContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;

        }
        public async Task<ServiceResponse<GetCharacterRequestDto>> AddWeapon(AddWeaponDto newWeapon)
        {
            var response = new ServiceResponse<GetCharacterRequestDto>();
            try
            {
                var character  = await _db.Characters.FirstOrDefaultAsync(c=>c.Id == newWeapon.CharacterId && c.User!.Id == int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!));
                if(character is null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }
                var weapon = new Weapon
                {
                    Name= newWeapon.Name,
                    Damage= newWeapon.Damage,
                    Character= character,
                };
                _db.Weapons.Add(weapon);
                await _db.SaveChangesAsync();
                response.Data = _mapper.Map<GetCharacterRequestDto>(character);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message= ex.Message;
            }
            return response;
        }
    }
}
