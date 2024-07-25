
using Microsoft.EntityFrameworkCore;

using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Persistence;

public class SkillsRepository(WssDbContext context) : ISkillsRepository
{
    public async Task<ICollection<Skill>> GetRandomSkills(int count)
    {
        return await context.Skills.AsQueryable().OrderBy(s => Guid.NewGuid()).Take(count).ToListAsync();
    }
}
