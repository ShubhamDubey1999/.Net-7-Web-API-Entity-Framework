global using AutoMapper;
using DOTNET_RPG.Models;
using System.Security.Claims;

namespace DOTNET_RPG.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        //private static List<Characters> characters = new List<Characters>
        //{
        //    new Characters(),
        //    new Characters{ Id = 1 , Name = "Sam"}

        //};
        private readonly IMapper _mapper;
        private readonly DataContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CharacterService(IMapper mapper, DataContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _db = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        public async Task<ServiceResponse<List<GetCharacterRequestDto>>> AddCharacter(AddCharacterResponseDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterRequestDto>>();
            var character = _mapper.Map<Characters>(newCharacter);
            character.User = await _db.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
            //character.Id = _db.Characters.Max(x => x.Id) + 1;
            _db.Characters.Add(character);
            await _db.SaveChangesAsync();
            serviceResponse.Data = await _db.Characters.Where(x => x.User!.Id == GetUserId()).Select(c => _mapper.Map<GetCharacterRequestDto>(c)).ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterRequestDto>>> DeleteCharacter(int Id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterRequestDto>>();
            try
            {
                var character = await _db.Characters.FirstOrDefaultAsync(c => c.Id == Id && c.User!.Id == GetUserId());
                if (character is null)
                    throw new ArgumentNullException($"Character with id '{Id}' not found");
                _db.Characters.Remove(character);
                //_mapper.Map(UpdatedCharacter, character);
                serviceResponse.Data = await _db.Characters.Where(c => c.User!.Id == GetUserId()).Select(x => _mapper.Map<GetCharacterRequestDto>(x)).ToListAsync();
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = e.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterRequestDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterRequestDto>>();
            var dbCharacters = await _db.Characters
                .Include(c=>c.Weapon)
                .Include(c=>c.Skills)
                .Where(c => c.User!.Id == GetUserId())
                .ToListAsync();
            //serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterRequestDto>(c)).ToList();
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterRequestDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterRequestDto>> GetCharactersById(int Id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterRequestDto>();
            var dbcharacter = await _db.Characters.FirstOrDefaultAsync(c => c.Id == Id && c.User!.Id == GetUserId());
            serviceResponse.Data = _mapper.Map<GetCharacterRequestDto>(dbcharacter);
            return serviceResponse;
            //if(character is not null)
            //    return character;
            //throw new Exception("Character not found...");
        }

        public async Task<ServiceResponse<GetCharacterRequestDto>> UpdateCharacter(UpdateCharacterDto UpdatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterRequestDto>();
            try
            {
                var character = await _db.Characters.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == UpdatedCharacter.Id);
                if (character is null || character.User!.Id != GetUserId())
                    throw new ArgumentNullException($"Character with id '{UpdatedCharacter.Id}' not found");
                character.Name = UpdatedCharacter.Name;
                character.Strength = UpdatedCharacter.Strength;
                character.Defense = UpdatedCharacter.Defense;
                character.Intelligence = UpdatedCharacter.Intelligence;
                character.Class = UpdatedCharacter.Class;
                //_mapper.Map(UpdatedCharacter, character);
                await _db.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetCharacterRequestDto>(character);
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = e.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterRequestDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            var response = new ServiceResponse<GetCharacterRequestDto>();
            try
            {
                var character = await _db.Characters.Include(c => c.Weapon).Include(c => c.Skills).FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId && c.User!.Id == GetUserId());
                if (character is null)
                {
                    response.Success = false; response.Message = "Character not found.";
                    return response;
                }
                var skill = await _db.Skills.FirstOrDefaultAsync(c=>c.Id == newCharacterSkill.SkillId);
                if (skill is null)
                {
                    response.Success = false; response.Message = "Skill not found.";
                    return response;
                }
                character.Skills!.Add(skill);
                await _db.SaveChangesAsync();
                response.Data = _mapper.Map<GetCharacterRequestDto>(character);
            }
            catch (Exception ex)
            {
                response.Success = false; response.Message = ex.Message;
            }
            return response;
        }
    }
}
