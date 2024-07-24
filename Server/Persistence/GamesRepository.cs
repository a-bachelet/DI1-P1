
using Microsoft.EntityFrameworkCore;

using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Persistence;

public class GamesRepository(WssDbContext context) : IGamesRepository
{
    public async Task<bool> GameExists(int gameId) => await context.Games.AnyAsync(game => game.Id == gameId);
    public async Task<bool> IsGameNameAvailable(string gameName) => !await context.Games.AnyAsync(game => game.Name == gameName);

    public async Task SaveGame(Game game)
    {
        await context.AddAsync(game);
        await context.SaveChangesAsync();
    }
}
