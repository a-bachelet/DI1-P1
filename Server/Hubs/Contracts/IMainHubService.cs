namespace Server.Hubs.Contracts;

public interface IMainHubService
{
    Task UpdateJoinableGamesList(IMainHubClient? caller = null);
}
