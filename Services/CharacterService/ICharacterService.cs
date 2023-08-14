namespace DOTNET_RPG.Services.CharacterService
{
    public interface ICharacterService
    {
        Task<ServiceResponse<List<GetCharacterRequestDto>>> GetAllCharacters();
        Task<ServiceResponse<GetCharacterRequestDto>> GetCharactersById(int Id);
        Task<ServiceResponse<List<GetCharacterRequestDto>>> AddCharacter(AddCharacterResponseDto newCharacter);
        Task<ServiceResponse<GetCharacterRequestDto>> UpdateCharacter(UpdateCharacterDto UpdatedCharacter);
        Task<ServiceResponse<List<GetCharacterRequestDto>>> DeleteCharacter(int Id);
        Task<ServiceResponse<GetCharacterRequestDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill);
    }
}
