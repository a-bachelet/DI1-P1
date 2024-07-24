using Server.Models;

namespace Server.Persistence.Contracts;

public interface IPlayersRepository
{
    Task<bool> IsPlayerNameAvailable(string playerName, int gameId);
    Task<bool> PlayerExists(int playerId);
    Task<Player?> GetById(int playerId);
    Task SavePlayer(Player player);
}
