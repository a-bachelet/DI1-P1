using Microsoft.AspNetCore.SignalR;

using Server.Hubs.Contracts;
using Server.Hubs.Records;
using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Hubs;

public class GameHub(IGameHubService gameHubService, IGamesRepository gamesRepository) : Hub<IGameHubClient>()
{
    public override async Task OnConnectedAsync()
    {
        var gameId = Context.GetHttpContext()?.GetRouteData().Values["gameId"];

        if (gameId is null) { Context.Abort(); return; }

        var game = await gamesRepository.GetById(int.Parse((string) gameId));

        if (game is null) { Context.Abort(); return; }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"{game.Id}__{game.Name}");

        await gameHubService.UpdateCurrentGame(Clients.Caller, game: game);

        await base.OnConnectedAsync();
    }
}

public class GameHubService(IHubContext<GameHub, IGameHubClient> gameHubContext, IGamesRepository gamesRepository) : IGameHubService
{
    public async Task UpdateCurrentGame(IGameHubClient? caller = null, int? gameId = null, Game? game = null)
    {
        if (game is null && gameId is null) { return; }

        game ??= await gamesRepository.GetById((int) gameId!);

        var data = new GameOverview(
            (int) game!.Id!, game.Name,
            game.Players.Select(p => new PlayerOverview((int) p.Id!, p.Name)).ToList(),
            game.Players.Count,
            3
        );

        caller ??= gameHubContext.Clients.Group($"{game.Id}__{game.Name}");

        await caller.CurrentGameUpdated(data);
    }
}
