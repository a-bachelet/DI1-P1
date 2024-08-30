using System.ComponentModel;
using System.Data;
using System.Net.Http.Json;

using Client.Records;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

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
    private bool CurrentGameStarted = false;

    public async Task Show()
    {
        await BeforeShow();
        await LoadGame();
        await DisplayMainView();
        await DisplayCompanyView();
    }

    private Task BeforeShow()
    {
        Target.RemoveAll();

        ReloadWindowTitle();

        return Task.CompletedTask;
    }

    private void ReloadWindowTitle()
    {
        var gameName = CurrentGame is null ? "..." : CurrentGame.Name;
        Target.Title = $"{MainWindow.Title} - [Game {gameName}]";
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

        hubConnection.On<GameOverview>("CurrentGameUpdated", async data =>
        {
            CurrentGame = data;
            ReloadWindowTitle();
            if (CurrentView is not null) { await CurrentView.Refresh(CurrentGame); };
            CurrentGameLoading = false;
        });

        await hubConnection.StartAsync();

        var loadingDialog = new Dialog()
        {
            Width = 17,
            Height = 3
        };

        var loadingText = new Label()
        {
            Text = "Loading game...",
            X = Pos.Center(),
            Y = Pos.Center()
        };

        loadingDialog.Add(loadingText);

        Target.Add(loadingDialog);

        while (CurrentGameLoading) { await Task.Delay(100); }

        Target.Remove(loadingDialog);
    }

    private async Task DisplayMainView()
    {
        Target.RemoveAll();

        var mainView = new CurrentGameMainView(CurrentGame!, PlayerName);

        CurrentView = mainView;

        mainView.X = mainView.Y = Pos.Center();
        mainView.OnStart = (_, __) => { CurrentGameStarted = true; };

        Target.Add(mainView);

        while (!CurrentGameStarted)
        {
            await Task.Delay(100);
        }
    }

    private async Task DisplayCompanyView()
    {
        Target.RemoveAll();

        var companyView = new CurrentGameCompanyView(CurrentGame!, PlayerName);

        CurrentView = companyView;

        companyView.X = companyView.Y = 5;
        companyView.Width = companyView.Height = Dim.Fill() - 5;

        Target.Add(companyView);

        while (true)
        {
            await Task.Delay(100);
        }
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
    private readonly FrameView Status = new();
    private readonly Button StartButton = new() { Text = "Start Game" };

    public EventHandler<HandledEventArgs> OnStart = (_, __) => { };

    public CurrentGameMainView(GameOverview game, string playerName)
    {
        Game = game;
        PlayerName = playerName;

        Width = Dim.Auto(DimAutoStyle.Auto);
        Height = Dim.Auto(DimAutoStyle.Auto);

        SetupPlayers();
        SetupStatus();
        SetupStartButton();
    }

    public override Task Refresh(GameOverview game)
    {
        Game = game;

        Remove(Players);
        Remove(Status);
        Remove(StartButton);

        SetupPlayers();
        SetupStatus();
        SetupStartButton();

        return Task.CompletedTask;
    }

    private void SetupPlayers()
    {
        Players.Title = $"Players ({Game.PlayersCount}/{Game.MaximumPlayersCount})";
        Players.X = 0;
        Players.Y = 0;
        Players.Width = 100;
        Players.Height = 6 + Game.Players.Count;
        Players.Enabled = false;

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
                player.Company.Name,
                $"{player.Company.Treasury} $",
                PlayerName == player.Name ? "⭐" : ""
            ]);
        }

        var dataTableSource = new DataTableSource(dataTable);

        var tableView = new TableView()
        {
            X = Pos.Center(),
            Y = Pos.Center(),
            Width = Game.Players.Max(p => p.Name.Length)
                + Game.Players.Max(p => p.Company.Name.Length)
                + Game.Players.Max(p => p.Company.Treasury.ToString().Length)
                + " $".Length
                + "⭐".Length
                + 6,
            Height = Dim.Fill(),
            Table = dataTableSource,
            Style = new TableStyle
            {
                ShowHorizontalBottomline = true,
                ExpandLastColumn = false,
            }
        };

        Players.Add(tableView);
    }

    private void SetupStatus()
    {
        Status.Title = "Status";
        Status.X = Pos.Left(Players);
        Status.Y = Pos.Bottom(Players) + 2;
        Status.Width = Players.Width;
        Status.Height = 3;

        Add(Status);

        var statusLabel = new Label() { Text = Game.Status is null ? "" : Game.Status, X = Pos.Center(), Y = Pos.Center() };

        Status.Add(statusLabel);
    }

    private void SetupStartButton()
    {
        if (PlayerName != Game.Players.First().Name) { return; }

        StartButton.X = Pos.Center();
        StartButton.Y = Pos.Bottom(Status) + 2;
        StartButton.Width = StartButton.Height = Dim.Auto(DimAutoStyle.Text);
        StartButton.Accept += async (_, __) => await StartGame();

        Add(StartButton);

        StartButton.SetFocus();
    }

    private async Task StartGame()
    {
        RemoveAll();

        var loadingDialog = new Dialog()
        {
            Width = 18,
            Height = 3
        };

        var loadingText = new Label()
        {
            Text = "Starting game...",
            X = Pos.Center(),
            Y = Pos.Center()
        };

        loadingDialog.Add(loadingText);

        Add(loadingDialog);

        var httpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true
        };

        var httpClient = new HttpClient(httpHandler)
        {
            BaseAddress = new Uri("https://localhost:7032"),
        };

        var request = httpClient.PostAsJsonAsync($"/games/{Game.Id}/start", new { });
        var response = await request;

        if (!response.IsSuccessStatusCode)
        {
            await Refresh(Game);
        }
        else
        {
            OnStart(null, new HandledEventArgs());
        }
    }
}

public class CurrentGameCompanyView : CurrentGameView
{
    private GameOverview Game;
    private PlayerOverview CurrentPlayer;
    private readonly string PlayerName;

    private readonly FrameView Player = new();
    private readonly FrameView Company = new();
    private readonly FrameView Treasury = new();
    private readonly FrameView Rounds = new();
    private readonly FrameView Employees = new();
    private readonly FrameView Actions = new();

    public CurrentGameCompanyView(GameOverview game, string playerName)
    {
        Game = game;
        PlayerName = playerName;
        CurrentPlayer = Game.Players.First(p => p.Name == PlayerName);

        Width = Dim.Auto(DimAutoStyle.Auto);
        Height = Dim.Auto(DimAutoStyle.Auto);

        SetupHeader();
        SetupBody();
    }

    public override Task Refresh(GameOverview game)
    {
        Game = game;
        CurrentPlayer = Game.Players.First(p => p.Name == PlayerName);

        Remove(Player);
        Remove(Company);
        Remove(Treasury);
        Remove(Rounds);
        Remove(Employees);
        Remove(Actions);

        SetupHeader();
        SetupBody();

        return Task.CompletedTask;
    }

    private void SetupHeader()
    {
        Player.Title = "Player";
        Company.Title = "Company";
        Treasury.Title = "Treasury";
        Rounds.Title = "Rounds";

        Player.Width = Dim.Percent(25);
        Company.Width = Dim.Percent(25);
        Treasury.Width = Dim.Percent(25);
        Rounds.Width = Dim.Percent(25);

        Player.Height = Dim.Auto(DimAutoStyle.Content);
        Company.Height = Dim.Auto(DimAutoStyle.Content);
        Treasury.Height = Dim.Auto(DimAutoStyle.Content);
        Rounds.Height = Dim.Auto(DimAutoStyle.Content);

        Player.X = 0;
        Company.X = Pos.Right(Player);
        Treasury.X = Pos.Right(Company);
        Rounds.X = Pos.Right(Treasury);

        Player.Y = 0;
        Company.Y = 0;
        Treasury.Y = 0;
        Rounds.Y = 0;

        Player.Add(new Label { Text = CurrentPlayer.Name });
        Company.Add(new Label { Text = CurrentPlayer.Company.Name });
        Treasury.Add(new Label { Text = $"{CurrentPlayer.Company.Treasury} $" });
        Rounds.Add(new Label { Text = $"{Game.CurrentRound}/{Game.MaximumRounds}" });

        Add(Player);
        Add(Company);
        Add(Treasury);
        Add(Rounds);
    }

    private void SetupBody()
    {
        Employees.Title = "Employees";
        Actions.Title = "Actions";

        Employees.Width = Dim.Percent(80);
        Actions.Width = Dim.Percent(20) - 2;

        Employees.Height = Dim.Fill();
        Actions.Height = Dim.Fill();

        Employees.X = 0;
        Actions.X = Pos.Right(Employees);

        Employees.Y = Pos.Bottom(Player);
        Actions.Y = Pos.Top(Employees);

        var employeesTree = new TreeView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            BorderStyle = LineStyle.Dotted
        };
        var employeesData = CurrentPlayer.Company.Employees.Select(e =>
        {
            var node = new TreeNode($"{e.Name} | {e.Salary} $");
            e.Skills.ToList().ForEach(s => node.Children.Add(
                new TreeNode($"{s.Name} | {s.Level}")
            ));
            return node;
        });

        employeesTree.AddObjects(employeesData);

        Employees.Add(employeesTree);

        employeesTree.ExpandAll();

        Add(Employees);
        Add(Actions);
    }
}
