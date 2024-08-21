using Server.Models;

namespace Server.Hubs.Contracts;

public interface IGameHubService
{
    Task UpdateCurrentGame(IGameHubClient? caller = null, int? gameId = null, Game? game = null);
}
