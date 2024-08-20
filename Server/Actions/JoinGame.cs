
using FluentResults;

using Microsoft.AspNetCore.SignalR;

using Server.Actions.Contracts;
using Server.Hubs;
using Server.Models;

namespace Server.Actions;

public sealed record JoinGameParams(
    string PlayerName,
    string CompanyName,
    int? GameId = null,
    Game? Game = null
) : CreatePlayerParams(PlayerName, CompanyName, GameId, Game);

public class JoinGameValidator : CreatePlayerValidator;

public class JoinGame(
    IAction<CreatePlayerParams,
    Result<Player>> createPlayerAction,
    IHubContext<GameHub> hubContext
) : IAction<JoinGameParams, Result<Player>>
{
    public async Task<Result<Player>> PerformAsync(JoinGameParams actionParams)
    {
        var createPlayerActionResult = await createPlayerAction.PerformAsync(actionParams);

        if (createPlayerActionResult.IsSuccess)
        {
            var player = createPlayerActionResult.Value;
            await hubContext.Clients.Group(player.Game.Name).SendAsync("GameJoined", player.Name);
        }

        return createPlayerActionResult;
    }
}
