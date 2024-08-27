using Terminal.Gui;

namespace Client.Screens;

public class TitleScreen(Window target)
{
    public Window Target { get; } = target;

    public async Task Show()
    {
        await BeforeShow();
        await ShowTitle();
    }

    private Task BeforeShow()
    {
        Target.RemoveAll();
        Target.Title = "";
        return Task.CompletedTask;
    }

    private async Task ShowTitle()
    {
        var titleLabel = new Label() { X = Pos.Center(), Y = Pos.Center() };

        Target.Add(titleLabel);

        var titleText = "Why So Serious ?";
        var displayedText = "";

        foreach (var character in titleText)
        {
            displayedText += character;
            titleLabel.Text = displayedText;
            await Task.Delay(100);
        }

        await Task.Delay(500);
    }
}
