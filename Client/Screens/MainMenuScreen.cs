using System;

using Client.Prompts;

namespace Client.Screens;

public static class MainMenuScreen
{
    public static async Task<bool> Show()
    {
        Console.Clear();

        var mainOption = MainMenuPrompt.Show();

        return mainOption switch
        {
            MainMenuPrompt.MainMenuOption.CREATE_GAME => await CreateGameScreen.Show(),
            MainMenuPrompt.MainMenuOption.JOIN_GAME => await JoinGameScreen.Show(),
            MainMenuPrompt.MainMenuOption.QUIT => true,
            _ => true,
        };
    }
}
