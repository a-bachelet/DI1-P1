using System;

using Spectre.Console;

namespace Client.Prompts;

public static class MainMenuPrompt
{
    public enum MainMenuOption
    {
        CREATE_GAME,
        JOIN_GAME,
        QUIT
    }

    public static MainMenuOption Show()
    {
        var selectionTitle = "What do you want to do ?";
        var selectionChoices = new[] { MainMenuOption.CREATE_GAME, MainMenuOption.JOIN_GAME, MainMenuOption.QUIT };

        static string selectionConverter(MainMenuOption option) => option switch
        {
            MainMenuOption.CREATE_GAME => "Create a game",
            MainMenuOption.JOIN_GAME => "Join a game",
            MainMenuOption.QUIT => "Quit",
            _ => throw new NotImplementedException(),
        };

        var selectionPrompt = new SelectionPrompt<MainMenuOption>()
            .Title(selectionTitle)
            .AddChoices(selectionChoices)
            .UseConverter(selectionConverter);

        return AnsiConsole.Prompt(selectionPrompt);
    }
}
