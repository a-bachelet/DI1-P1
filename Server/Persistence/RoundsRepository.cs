
using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Persistence;

public class RoundsRepository(WssDbContext context) : IRoundsRepository
{
    public async Task<Round?> GetById(int roundId)
    {
        return await context.Rounds.FindAsync(roundId);
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
