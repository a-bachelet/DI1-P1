
using Microsoft.EntityFrameworkCore;

using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Persistence;

public class GamesRepository(WssDbContext context) : IGamesRepository
{
    public async Task<bool> GameExists(int gameId)
    {
        return await context.Games.AnyAsync(game => game.Id == gameId);
    }

    public async Task<bool> IsGameNameAvailable(string gameName)
    {
        return !await context.Games.AnyAsync(game => game.Name == gameName);
    }

    public async Task<Game?> GetById(int gameId)
    {
        return await context.Games.FindAsync(gameId);
    }

    public async Task SaveGame(Game game)
    {
        if (game.Id is null) {
            await context.AddAsync(game);
        }
        
        await context.SaveChangesAsync();
    }
}
