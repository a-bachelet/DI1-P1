using System.ComponentModel;
using System.Data;
using System.Xml;

using Client.Records;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

using Spectre.Console;

using Terminal.Gui;

namespace Client.Screens;

public class CurrentGameScreen(Window target, int gameId, string playerName)
{
    private readonly Window Target = target;
    private CurrentGameView? CurrentView = null;

    private readonly int GameId = gameId;
    private readonly string PlayerName = playerName;
    private GameOverview? CurrentGame = null;

    private bool CurrentGameLoading = true;

    public async Task Show()
    {
        await BeforeShow();
        await LoadGame();
        await DisplayMainView();

        await Task.Delay(1000000000);
    }

    private Task BeforeShow()
    {
        Target.RemoveAll();

        var gameName = CurrentGame is null ? "..." : CurrentGame.Name;

        Target.Title = $"{MainWindow.Title} - [Game {gameName}]";

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

        hubConnection.On<GameOverview>("CurrentGameUpdated", async data => {
            CurrentGame = data;
            if (CurrentView is not null) { await CurrentView.Refresh(CurrentGame); };
            CurrentGameLoading = false;
        });

        await hubConnection.StartAsync();

        var loadingDialog = new Dialog() {
            Width = 17, Height = 3
        };

        var loadingText = new Label() {
            Text = "Loading game...", X = Pos.Center(), Y = Pos.Center()
        };

        loadingDialog.Add(loadingText);

        Target.Add(loadingDialog);

        while (CurrentGameLoading) { await Task.Delay(100); }

        Target.Remove(loadingDialog);
    }

    private Task DisplayMainView()
    {
        var mainView = new CurrentGameMainView(CurrentGame!, PlayerName);

        CurrentView = mainView;

        mainView.X = mainView.Y = 0;
        mainView.Width = mainView.Height = Dim.Fill();

        Target.Add(mainView);

        return Task.CompletedTask;
    }
}

public abstract class CurrentGameView : View
{
    public abstract Task Refresh(GameOverview game);
}

public class CurrentGameMainView : CurrentGameView
{
    private GameOverview Game;
    private readonly string PlayerName;

    private readonly FrameView Players = new();

    public CurrentGameMainView(GameOverview game, string playerName)
    {
        Game = game;
        PlayerName = playerName;

        SetupPlayers();
    }

    public override Task Refresh(GameOverview game)
    {
        Game = game;

        Remove(Players);
        SetupPlayers();

        return Task.CompletedTask;
    }

    private void SetupPlayers()
    {
        Players.Title = $"Players ({Game.PlayersCount}/{Game.MaximumPlayersCount})";
        Players.X = Pos.Center();
        Players.Y = Pos.Center();
        Players.Width = Dim.Auto(DimAutoStyle.Content);
        Players.Height = 6 + Game.Players.Count;

        Add(Players);

        var dataTable = new DataTable();

        dataTable.Columns.Add("Name");
        dataTable.Columns.Add("Company");
        dataTable.Columns.Add("Treasury");
        dataTable.Columns.Add("⭐");

        foreach (var player in Game.Players)
        {
            dataTable.Rows.Add([
                player.Name,
                "My Company",
                "1.000.000$",
                PlayerName == player.Name ? "⭐" : ""
            ]);
        }

        var dataTableSource = new DataTableSource(dataTable);

        var tableView = new TableView() {
            X = 0, Y= 0,
            Width = Game.Players.Max(p => p.Name.Length) + "My Company".Length + "1.000.000$".Length + "⭐".Length + 6,
            Height = Dim.Fill(),
            Table = dataTableSource,
            Style = new TableStyle {
                ShowHorizontalBottomline = true,
                ExpandLastColumn = false,
            }
        };

        Players.Add(tableView);
    }
}
