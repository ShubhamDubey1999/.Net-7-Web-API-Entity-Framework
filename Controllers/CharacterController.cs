using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DOTNET_RPG.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        //private static List<Characters> characters = new List<Characters>
        //{
        //    new Characters(),
        //    new Characters{ Id = 1 , Name = "Sam"}

        //};
        private readonly ICharacterService _characterService;
        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }
        //[AllowAnonymous]   // to protect from Authorization
        [HttpGet("GetAll")]
        //[Route("api/GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterRequestDto>>>> Get()
        {
            return Ok( await _characterService.GetAllCharacters());
        }
        [HttpGet("{Id}")]
        //[Route("api/SingleCharacter")]
        public async Task<ActionResult<ServiceResponse<GetCharacterRequestDto>>> GetSingle(int Id)
        {
            return Ok( await _characterService.GetCharactersById(Id));
        }
        [HttpPost]
        public async Task<ActionResult<List<ServiceResponse<GetCharacterRequestDto>>>> AddCharacter(AddCharacterResponseDto newCharacter)
        {
            return Ok(await _characterService.AddCharacter(newCharacter));
        }
        [HttpPut]
        public async Task<ActionResult<List<ServiceResponse<GetCharacterRequestDto>>>> UpdateCharacter(UpdateCharacterDto UpdatedCharacter)
        {
            var response = _characterService.UpdateCharacter(UpdatedCharacter);
            if (response is null)
                return NotFound(response);
            return Ok(response);
        }
        [HttpDelete("{Id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterRequestDto>>> DeleteCharacter(int Id)
        {
            var response = await _characterService.DeleteCharacter(Id);
            if(response.Data is null)
                return NotFound(response);
            return Ok(response);
        }
        [HttpPost("Skill")]
        public async Task<ActionResult<ServiceResponse<GetCharacterRequestDto>>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            return Ok(await _characterService.AddCharacterSkill(newCharacterSkill));
        }
    }
}
