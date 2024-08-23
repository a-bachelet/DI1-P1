using Spectre.Console;

namespace Client.Screens;

public class TitleScreen
{
    public static async Task<bool> Show()
    {
        Console.Clear();

        var titlePanel = new Panel("");

        await AnsiConsole.Live(titlePanel).StartAsync(async ctx =>
        {
            var titleText = "Why So Serious ?";
            var displayedText = "";

            foreach (var character in titleText)
            {
                displayedText += character;
                titlePanel = new Panel(new FigletText(displayedText).Centered().Color(Color.Red));
                ctx.UpdateTarget(titlePanel);
                ctx.Refresh();

                await Task.Delay(100);
            }

            await Task.Delay(500);
        });

        return false;
    }
}
