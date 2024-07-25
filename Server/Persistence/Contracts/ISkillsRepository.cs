using Server.Models;

namespace Server.Persistence.Contracts;

public interface ISkillsRepository
{
    Task<ICollection<Skill>> GetRandomSkills(int count);
}
