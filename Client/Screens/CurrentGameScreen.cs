using Client.Records;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

using Spectre.Console;

using Terminal.Gui;

namespace Client.Screens;

public class CurrentGameScreen(Window target, int gameId, string playerName)
{
    private readonly Window Target = target;

    private readonly int GameId = gameId;

    private readonly string PlayerName = playerName;

    private bool Loading = true;

    private GameOverview? CurrentGame = null;

    public async Task Show()
    {
        await BeforeShow();
        await LoadGame();

        AnsiConsole.Clear();
        AnsiConsole.WriteLine(CurrentGame!.Id!.ToString()!);
        AnsiConsole.WriteLine(CurrentGame!.Name!.ToString()!);
        AnsiConsole.WriteLine(CurrentGame!.Players!.ToString()!);

        await Task.Delay(5000);
    }

    private Task BeforeShow()
    {
        Target.RemoveAll();
        Target.Title = $"{MainWindow.Title} - [Current Game]";
        return Task.CompletedTask;
    }

    private async Task LoadGame()
    {
        var hubConnection = new HubConnectionBuilder()
            .WithUrl(new Uri($"wss://localhost:7032/games/{GameId}"), opts =>
            {
                opts.HttpMessageHandlerFactory = (message) =>
                {
                    if (message is HttpClientHandler clientHandler)
                    {
                        clientHandler.ServerCertificateCustomValidationCallback +=
                            (sender, certificate, chain, sslPolicyErrors) => { return true; };
                    }

                    return message;
                };
            })
            .AddJsonProtocol()
            .Build();

        hubConnection.On<GameOverview>("CurrentGameUpdated", data => {
            CurrentGame = data;
            Loading = false;
        });

        await hubConnection.StartAsync();

        var loadingDialog = new Dialog("", 17, 3);
        var loadingText = new Label("Loading game...");

        loadingDialog.Add(loadingText);

        Target.Add(loadingDialog);

        while (Loading) { await Task.Delay(100); }

        Target.Remove(loadingDialog);
    }
}
