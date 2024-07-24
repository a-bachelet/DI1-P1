using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Persistence;

public class PlayersRepository(WssDbContext context) : IPlayersRepository
{
    public async Task<bool> IsPlayerNameAvailable(string playerName, int gameId) =>
      !await context.Players.AnyAsync(player => player.Name == playerName && player.GameId == gameId);

    public async Task SavePlayer(Player player)
    {
        await context.AddAsync(player);
        await context.SaveChangesAsync();
    }
}
