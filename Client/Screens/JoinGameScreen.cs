using System.Collections;
using System.Net.Http.Json;
using System.Text.Json;

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
    private readonly Button ListReturnButton = new("Return");
    private readonly JoinGameForm Form = new();

    private ICollection<JoinableGame> _joinableGames = [];
    private ICollection<JoinableGame> JoinableGames
    {
        get => _joinableGames;
        set { _joinableGames = value; ReloadListData(); }
    }
    private int? GameId = null;

    private bool Loading = true;
    private bool Errored = false;
    private bool ListReturned = false;
    private bool FormReturned = false;
    private bool FormSubmitted = false;

    public async Task Show()
    {
        await BeforeShow();
        await LoadGames();
        await SelectGame();

        if (ListReturned) {
            await Return();
            return;
        }

        await DisplayForm();

        if (FormReturned) {
            await SelectGame();
            return;
        }

        await JoinGame();

        if (Errored)
        {
            FormSubmitted = false;
            FormReturned = false;

            await DisplayForm(true);

            if (FormReturned) {
                await SelectGame();
                return;
            }

            return;
        }

        var currentGameScreen = new CurrentGameScreen(Target, (int) GameId!, Form.PlayerNameField.Text.ToString()!);

        await currentGameScreen.Show();
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
        GamesList.X = GamesList.Y = Pos.Center();
        GamesList.Width = JoinableGames.Max(g => g.Name.Length);
        GamesList.Height = Math.Min(JoinableGames.Count, 20);

        ListReturnButton.X = Pos.Center();
        ListReturnButton.Y = Pos.Bottom(GamesList) + 1;
        ListReturnButton.Clicked += () => ListReturned = true;

        Target.Add(GamesList);
        Target.Add(ListReturnButton);

        GamesList.OpenSelectedItem += (selected) => GameId = ((JoinableGame) selected.Value).Id;

        while (GameId is null && !ListReturned) { await Task.Delay(100); };
    }

    private async Task DisplayForm(bool errored = false)
    {
        Target.RemoveAll();

        Form.OnReturn = () => FormReturned = true;
        Form.OnSubmit = () => FormSubmitted = true;

        Form.FormView.X = Form.FormView.Y = Pos.Center();
        Form.FormView.Width = 50;
        Form.FormView.Height = 9;

        Target.Add(Form.FormView);

        while (!FormReturned && !FormSubmitted) {
            await Task.Delay(100);
        }
    }

    private async Task JoinGame()
    {
        Target.Remove(Form.FormView);

        var loadingDialog = new Dialog("", 17, 3);
        var loadingText = new Label("Joining game...");

        loadingDialog.Add(loadingText);

        Target.Add(loadingDialog);

        var httpHandler = new HttpClientHandler {
            ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true
        };

        var httpClient = new HttpClient(httpHandler) {
            BaseAddress = new Uri("https://localhost:7032"),
        };

        var playerName = Form.PlayerNameField.Text.ToString();
        var companyName = Form.CompanyNameField.Text.ToString();

        var requestBody = new {playerName, companyName};
        var request = httpClient.PostAsJsonAsync($"/games/{GameId}/join", requestBody);
        var response = await request;

        if (!response.IsSuccessStatusCode)
        {
            Errored = true;
        }
        else
        {
            try {
                var content = await response.Content.ReadFromJsonAsync<JsonElement>();
            }
            catch(Exception e) {
                AnsiConsole.Clear();
                AnsiConsole.Write(new Markup("[red]Error: An error occured[/]\n"));
                AnsiConsole.WriteException(e);
            }
        }
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

public class JoinGameForm
{
    private Action _onSubmit = () => {};
    private Action _onReturn = () => {};

    public Action OnSubmit
    {
        get => _onSubmit;
        set
        {
            SubmitButton.Clicked -= _onSubmit;
            SubmitButton.Clicked += value;
            _onSubmit = value;
        }
    }
    public Action OnReturn
    {
        get => _onReturn;
        set
        {
            ReturnButton.Clicked -= _onReturn;
            ReturnButton.Clicked += value;
            _onReturn = value;
        }
    }

    public View FormView { get; }
    public View ButtonsView { get; }
    public Button SubmitButton { get; }
    public Button ReturnButton { get; }
    public Label PlayerNameLabel { get; }
    public Label CompanyNameLabel { get; }
    public TextField PlayerNameField { get; }
    public TextField CompanyNameField { get; }

    public JoinGameForm()
    {
        PlayerNameLabel = new Label("Player name :") {
            X = 0, Y = 0, Width = 20
        };

        CompanyNameLabel = new Label("Company name :") {
            X = Pos.Left(PlayerNameLabel), Y = Pos.Bottom(PlayerNameLabel) + 1, Width = 20
        };

        PlayerNameField = new TextField("") {
            X = Pos.Right(PlayerNameLabel), Y = Pos.Top(PlayerNameLabel), Width = Dim.Fill()
        };

        CompanyNameField = new TextField("") {
            X = Pos.Right(CompanyNameLabel), Y = Pos.Top(CompanyNameLabel), Width = Dim.Fill()
        };

        ButtonsView = new View() {
            Width = 1, Height = 1, X = Pos.Center(), Y = Pos.Bottom(CompanyNameLabel) + 1
        };

        SubmitButton = new Button() {
            Text = "Submit", IsDefault = true
        };

        ReturnButton = new Button() {
            Text = "Return", IsDefault = false, X = Pos.Right(SubmitButton) + 1
        };

        SubmitButton.Clicked += OnSubmit;
        ReturnButton.Clicked += OnReturn;

        ButtonsView.Add(SubmitButton, ReturnButton);

        SubmitButton.GetCurrentWidth(out var submitButtonWidth);
        ReturnButton.GetCurrentWidth(out var returnButtonWidth);

        ButtonsView.Width = submitButtonWidth + returnButtonWidth + 1;

        FormView = new View() {
            Width = Dim.Fill(), Height = Dim.Fill()
        };

        FormView.Add(
            PlayerNameLabel, CompanyNameLabel,
            PlayerNameField, CompanyNameField,
            ButtonsView
        );
    }
}
