using Server.Hubs.Records;

namespace Server.Hubs.Contracts;

public interface IGameHubClient
{
    Task CurrentGameUpdated(GameOverview data);
}
