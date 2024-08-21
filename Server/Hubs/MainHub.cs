using Microsoft.AspNetCore.SignalR;

using Server.Hubs.Contracts;
using Server.Hubs.Records;
using Server.Persistence.Contracts;

namespace Server.Hubs;

public class MainHub(IMainHubService mainHubService) : Hub<IMainHubClient>
{
    public override async Task OnConnectedAsync()
    {
        await mainHubService.UpdateJoinableGamesList(Clients.Caller);
        await base.OnConnectedAsync();
    }
}

public class MainHubService(IHubContext<MainHub, IMainHubClient> mainHubContext, IGamesRepository gamesRepository) : IMainHubService
{
    public async Task UpdateJoinableGamesList(IMainHubClient? caller = null)
    {
        var data = (await gamesRepository.GetJoinable())
            .Select(x => new JoinableGame((int) x.Id!, x.Name, x.Players.Count, 3))
            .ToList();

        caller ??= mainHubContext.Clients.All;

        await caller.JoinableGamesListUpdated(data);
    }
}
