using DOTNET_RPG.Dtos.Fight;

namespace DOTNET_RPG.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _db;
        private readonly IMapper _mapper;
        public FightService(DataContext dataContext, IMapper mapper)
        {
            _db = dataContext;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            var response = new ServiceResponse<FightResultDto>
            {
                Data = new FightResultDto()
            };
            try
            {
                var characters = await _db.Characters.Include(c => c.Skills).Include(c => c.Weapon).Where(c => request.CharcatersIds.Contains(c.Id)).ToListAsync();
                bool deafeated = false;
                while (!deafeated)
                {
                    foreach (var attacker in characters)
                    {
                        var opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                        var opponent = opponents[new Random().Next(opponents.Count)];
                        int damage = 0;
                        string attackUsed = string.Empty;
                        bool useWeapon = new Random().Next(2) == 0;
                        if (useWeapon && attacker.Weapon is not null)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, opponent);
                        }
                        else if (!useWeapon && attacker.Skills is not null)
                        {
                            var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            attackUsed = skill.Name;
                            damage = DoSkillAttack(attacker, opponent, skill);
                        }
                        else
                        {
                            response.Data.Log.Add($"{attacker.Name} wasn't able to attack!");
                            continue;
                        }
                        response.Data.Log.Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage");
                        if (opponent.Hitpoints <= 0)
                        {
                            deafeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has been defeated!");
                            response.Data.Log.Add($"{attacker.Name} wins with {attacker.Hitpoints} HP left!");
                            break;
                        }
                    }
                }
                characters.ForEach(x =>
                {
                    x.Fights++;
                    x.Hitpoints = 100;
                });
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();
            try
            {
                var attacker = await _db.Characters.Include(c => c.Skills).FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                var opponent = await _db.Characters.FirstOrDefaultAsync(c => c.Id == request.OpponentId);
                if (attacker is null || opponent is null || attacker.Skills is null)
                    throw new ArgumentException("Something is fishy is going on here...");

                var skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);
                if (skill is null)
                {
                    response.Success = false;
                    response.Message = $"{attacker.Name} doesn't know that skill!";
                    return response;
                }

                int damage = DoSkillAttack(attacker, opponent, skill);
                if (opponent.Hitpoints <= 0) response.Message = $"{opponent.Name} has been defeated!";
                await _db.SaveChangesAsync();
                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.Hitpoints,
                    OpponentHP = opponent.Hitpoints,
                    Damage = damage,
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        private static int DoSkillAttack(Characters attacker, Characters opponent, Skill skill)
        {
            int damage = skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= new Random().Next(opponent.Defeats);
            if (damage > 0) opponent.Hitpoints -= damage;
            return damage;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();
            try
            {
                var attacker = await _db.Characters.Include(c => c.Weapon).FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                var opponent = await _db.Characters.FirstOrDefaultAsync(c => c.Id == request.OpponentId);
                if (attacker is null || opponent is null || attacker.Weapon is null)
                    throw new ArgumentException("Something is fishy is going on here...");
                int damage = DoWeaponAttack(attacker, opponent);
                if (opponent.Hitpoints <= 0) response.Message = $"{opponent.Name} has been defeated!";
                await _db.SaveChangesAsync();
                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.Hitpoints,
                    OpponentHP = opponent.Hitpoints,
                    Damage = damage,
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        private static int DoWeaponAttack(Characters attacker, Characters opponent)
        {
            if (attacker.Weapon is null)
                throw new ArgumentException("Attacker has no weapon!");
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            damage -= new Random().Next(opponent.Defeats);
            if (damage > 0)
                opponent.Hitpoints -= damage;
            return damage;
        }

        public async Task<ServiceResponse<List<HighScoreDto>>> HighScore()
        {
            var characters = await _db.Characters.Where(c => c.Fights > 0).OrderByDescending(c => c.Victories).ThenBy(c => c.Defeats).ToListAsync();
            var response = new ServiceResponse<List<HighScoreDto>>
            {
                Data = characters.Select(c => _mapper.Map<HighScoreDto>(c)).ToList()
            };
            return response;
        }
    }
}
