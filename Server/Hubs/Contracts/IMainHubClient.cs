using Server.Hubs.Records;

namespace Server.Hubs.Contracts;

public interface IMainHubClient
{
    Task JoinableGamesListUpdated(ICollection<JoinableGame> data);
}
