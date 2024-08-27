using System.Collections;

using Client.Records;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

using Spectre.Console;

using Terminal.Gui;

namespace Client.Screens;

public class JoinGameScreen(Window target)
{
    private Window Target { get; } = target;
    private readonly ListView GamesList = new();

    private ICollection<JoinableGame> _joinableGames = [];
    private ICollection<JoinableGame> JoinableGames
    {
        get => _joinableGames;
        set { _joinableGames = value; ReloadListData(); }
    }
    private int? GameId = null;

    private bool Loading = true;
    private bool Returned = false;

    public async Task Show()
    {
        await BeforeShow();
        await LoadGames();
        await SelectGame();

        if (Returned) {
            await Return();
            return;
        }

        AnsiConsole.Clear();
        AnsiConsole.WriteLine(GameId!.ToString()!);

        await Task.Delay(5000);
    }

    private Task BeforeShow()
    {
        Target.RemoveAll();
        Target.Title = $"{MainWindow.Title} - [Join Game]";
        return Task.CompletedTask;
    }

    private async Task Return()
    {
        var mainMenuScreen = new MainMenuScreen(Target);
        await mainMenuScreen.Show();
    }

    private void ReloadListData()
    {
        var dataSource = new JoinGameChoiceListDataSource();
        dataSource.AddRange(JoinableGames);
        dataSource.Add(new JoinableGame(0, "", 0, 0));
        GamesList.Source = dataSource;
    }

    private async Task LoadGames()
    {
        var hubConnection = new HubConnectionBuilder()
            .WithUrl(new Uri($"wss://localhost:7032/main"), opts =>
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

        hubConnection.On<ICollection<JoinableGame>>("JoinableGamesListUpdated", data => {
            JoinableGames = data;
            Loading = false;
        });

        await hubConnection.StartAsync();

        var loadingDialog = new Dialog("", 18, 3);
        var loadingText = new Label("Loading games...");

        loadingDialog.Add(loadingText);

        Target.Add(loadingDialog);

        while (Loading) { await Task.Delay(100); }

        Target.Remove(loadingDialog);
    }

    private async Task SelectGame()
    {
        GamesList.X = GamesList.Y = 0;
        GamesList.Width = GamesList.Height = Dim.Fill();

        Target.Add(GamesList);
        GamesList.OpenSelectedItem += (selected) => {
            GameId = ((JoinableGame) selected.Value).Id;
            if (GameId == 0) { Returned = true; }
        };

        while (GameId is null) { await Task.Delay(100); };
    }
}

public class JoinGameChoiceListDataSource : List<JoinableGame>, IListDataSource
{
    public int Length => Count;

    public bool IsMarked(int item)
    {
        return false;
    }

    public void Render(ListView container, ConsoleDriver driver, bool selected, int item, int col, int line, int width, int start = 0)
    {
        if (item == Count - 1)
        {
            driver.AddStr("⬅️ Return");
        }
        else
        {
            var game = this[item];
            driver.AddStr($"{game.Name} ({game.PlayersCount}/{game.MaximumPlayersCount})");
        }
    }

    public void SetMark(int item, bool value) {}

    public IList ToList()
    {
        return this;
    }
}
