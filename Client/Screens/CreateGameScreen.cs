using System.Net.Http.Json;
using System.Text.Json;

using Spectre.Console;

using Terminal.Gui;

namespace Client.Screens;

public class CreateGameScreen(Window target)
{
    private Window Target { get; } = target;
    private readonly CreateGameForm Form = new();

    private int? GameId = null;

    private bool Submitted = false;
    private bool Returned = false;
    private bool Errored = false;

    public async Task Show()
    {
        await BeforeShow();
        await DisplayForm();

        if (Returned) {
            await Return();
            return;
        }

        await CreateGame();

        if (Errored) {
            Submitted = false;
            Returned = false;

            await DisplayForm(true);

            if (Returned) {
                await Return();
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
        Target.Title = $"{MainWindow.Title} - [Create Game]";
        return Task.CompletedTask;
    }

    private async Task Return()
    {
        var mainMenuScreen = new MainMenuScreen(Target);
        await mainMenuScreen.Show();
    }

    private async Task DisplayForm(bool errored = false)
    {
        Form.OnReturn = () => Returned = true;
        Form.OnSubmit = () => Submitted = true;

        Form.FormView.X = Form.FormView.Y = Pos.Center();
        Form.FormView.Width = 50;
        Form.FormView.Height = 9;

        Target.Add(Form.FormView);

        while (!Returned && !Submitted) {
            await Task.Delay(100);
        }
    }

    private async Task CreateGame()
    {
        Target.Remove(Form.FormView);

        var loadingDialog = new Dialog("", 18, 3);
        var loadingText = new Label("Creating game...");

        loadingDialog.Add(loadingText);

        Target.Add(loadingDialog);

        var httpHandler = new HttpClientHandler {
            ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true
        };

        var httpClient = new HttpClient(httpHandler) {
            BaseAddress = new Uri("https://localhost:7032"),
        };

        var gameName = Form.GameNameField.Text.ToString();
        var playerName = Form.PlayerNameField.Text.ToString();
        var companyName = Form.CompanyNameField.Text.ToString();
        var rounds = int.Parse(Form.RoundsField.Text.ToString()!);

        var requestBody = new {gameName, playerName, companyName, rounds};
        var request = httpClient.PostAsJsonAsync("/games", requestBody);
        var response = await request;

        if (!response.IsSuccessStatusCode)
        {
            Errored = true;
        }
        else
        {
            try {
                var content = await response.Content.ReadFromJsonAsync<JsonElement>();
                GameId = content.GetProperty("id").GetInt32();
            }
            catch(Exception e) {
                AnsiConsole.Clear();
                AnsiConsole.Write(new Markup("[red]Error: An error occured[/]\n"));
                AnsiConsole.WriteException(e);
            }
        }
    }
}

public class CreateGameForm
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
    public Label GameNameLabel { get; }
    public Label PlayerNameLabel { get; }
    public Label CompanyNameLabel { get; }
    public Label RoundsLabel { get; }
    public TextField GameNameField { get; }
    public TextField PlayerNameField { get; }
    public TextField CompanyNameField { get; }
    public TextField RoundsField { get; }

    public CreateGameForm()
    {
        GameNameLabel = new Label("Game name :") {
            X = 0, Y = 0, Width = 20
        };

        PlayerNameLabel = new Label("Player name :") {
            X = Pos.Left(GameNameLabel), Y = Pos.Bottom(GameNameLabel) + 1, Width = 20
        };

        CompanyNameLabel = new Label("Company name :") {
            X = Pos.Left(PlayerNameLabel), Y = Pos.Bottom(PlayerNameLabel) + 1, Width = 20
        };

        RoundsLabel = new Label("Rounds (15 - 100) :") {
            X = Pos.Left(CompanyNameLabel), Y = Pos.Bottom(CompanyNameLabel) + 1, Width = 20
        };

        GameNameField = new TextField("") {
            X = Pos.Right(GameNameLabel), Y = Pos.Top(GameNameLabel), Width = Dim.Fill()
        };

        PlayerNameField = new TextField("") {
            X = Pos.Right(PlayerNameLabel), Y = Pos.Top(PlayerNameLabel), Width = Dim.Fill()
        };

        CompanyNameField = new TextField("") {
            X = Pos.Right(CompanyNameLabel), Y = Pos.Top(CompanyNameLabel), Width = Dim.Fill()
        };

        RoundsField = new TextField("") {
            X = Pos.Right(RoundsLabel), Y = Pos.Top(RoundsLabel), Width = Dim.Fill()
        };

        ButtonsView = new View() {
            Width = 1, Height = 1, X = Pos.Center(), Y = Pos.Bottom(RoundsLabel) + 1
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
            GameNameLabel, PlayerNameLabel, CompanyNameLabel, RoundsLabel,
            GameNameField, PlayerNameField, CompanyNameField, RoundsField,
            ButtonsView
        );
    }
}
