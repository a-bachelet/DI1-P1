using System.Text.Json;

using Microsoft.AspNetCore.SignalR;

using Server.Persistence.Contracts;

namespace Server.Hubs;

public class GameHub(IGamesRepository gamesRepository) : Hub()
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiveMessage", "HELLO WORLD !");
        await base.OnConnectedAsync();
    }

    public async Task WatchGame(int gameId)
    {
        var game = await gamesRepository.GetById(gameId);

        if (game is null)
        {
            await Clients.Caller.SendAsync("Error", "WatchGame", $"Game with Id \"{gameId}\" not found.");
        }
        else
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, game.Name);
            await Clients.Caller.SendAsync("Game", JsonSerializer.Serialize(new { game.Name }));
        }
    }
}
