
using Microsoft.EntityFrameworkCore;

using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Persistence;

public class RoundsRepository(WssDbContext context) : IRoundsRepository
{
    public async Task<Round?> GetById(int roundId)
    {
        return await context.Rounds
            .Include(r => r.Game)
            .ThenInclude(g => g.Players)
            .Include(r => r.Game)
            .ThenInclude(g => g.RoundsCollection)
            .Include(r => r.Actions)
            .Where(r => r.Id == roundId)
            .FirstOrDefaultAsync();
    }

    public async Task SaveRound(Round round)
    {
        if (round.Id is null)
        {
            await context.AddAsync(round);
        }

        await context.SaveChangesAsync();
    }
}
