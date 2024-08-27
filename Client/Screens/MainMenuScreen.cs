using System.Collections;

using Terminal.Gui;

namespace Client.Screens;

public class MainMenuScreen(Window target)
{
    public Window Target { get; } = target;

    public MainMenuActionList.Action? Action { get; private set; } = null;

    public async Task Show()
    {
        await BeforeShow();
        await SelectAction();

        var next = Action switch {
            MainMenuActionList.Action.CREATE_GAME => new CreateGameScreen(Target).Show(),
            MainMenuActionList.Action.JOIN_GAME => new JoinGameScreen(Target).Show(),
            MainMenuActionList.Action.QUIT => Task.Run(() => Application.RequestStop()),
            _ => Task.Run(() => Application.RequestStop())
        };

        await next;
    }

    private Task BeforeShow()
    {
        Target.RemoveAll();
        Target.Title = $"{MainWindow.Title} - [Main Menu]";
        return Task.CompletedTask;
    }

    private async Task SelectAction()
    {
        var actionList = new MainMenuActionList() {
            X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill()
        };

        Target.Add(actionList);
        actionList.OpenSelectedItem += (selected) => { Action = (MainMenuActionList.Action) selected.Value; };
        while (Action is null) { await Task.Delay(100); };
    }
}

public class MainMenuActionList : ListView
{
    public enum Action {
        CREATE_GAME,
        JOIN_GAME,
        QUIT
    }

    private readonly MainMenuActionListDataSource Actions = [
        Action.CREATE_GAME,
        Action.JOIN_GAME,
        Action.QUIT
    ];

    public MainMenuActionList()
    {
        Source = Actions;
    }
}

public class MainMenuActionListDataSource : List<MainMenuActionList.Action>, IListDataSource
{
    public int Length => Count;

    public bool IsMarked(int item)
    {
        return false;
    }

    public void Render(ListView container, ConsoleDriver driver, bool selected, int item, int col, int line, int width, int start = 0)
    {
        switch (item) {
            case (int) MainMenuActionList.Action.CREATE_GAME:
                driver.AddStr("Create a game");
                break;
            case (int) MainMenuActionList.Action.JOIN_GAME:
                driver.AddStr("Join a game");
                break;
            case (int) MainMenuActionList.Action.QUIT:
                driver.AddStr("Quit");
                break;
        }
    }

    public void SetMark(int item, bool value) {}

    public IList ToList()
    {
        return this;
    }
}