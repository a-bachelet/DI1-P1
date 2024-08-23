using Client.Records;

using Spectre.Console;

namespace Client.Prompts;

public static class JoinGamePrompt
{
    static async Task DisplayFetchStatus(ICollection<JoinableGame> games)
    {
        await AnsiConsole.Status().StartAsync("Fetching games...", async ctx =>
        {
            while (games.Count == 0)
            {
                await Task.Delay(0);
            }
        });
    }

    static async Task DisplayNoGameError(ICollection<JoinableGame> games)
    {
        var noGameError = new Markup("[red]Error: No game is available[/]");

        await AnsiConsole.Live(noGameError).StartAsync(async ctx =>
        {
            ctx.Refresh();

            while (games.Count == 0 || games.FirstOrDefault() is null)
            {
                await Task.Delay(100);
            }

            ctx.UpdateTarget(new Text(""));
            ctx.Refresh();
        });
    }

    public static async Task<JoinableGame> Show(ICollection<JoinableGame> games)
    {
        await DisplayFetchStatus(games);
        await DisplayNoGameError(games);


        var gamesSelection =
            new SelectionPrompt<JoinableGame>()
                .UseConverter(game => game.Name)
                .AddChoices(games)
                .Title("[blue underline]Choose a game :[/]");

        return AnsiConsole.Prompt(gamesSelection);
    }
}
