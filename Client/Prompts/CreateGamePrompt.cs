using System;

using Spectre.Console;

namespace Client.Prompts;

public class CreateGamePrompt
{
    static string AskForGameName()
    {
        var gameName = AnsiConsole.Prompt(new TextPrompt<string>("Enter game name :"));

        if (gameName is null or "") {
            gameName = AskForGameName();
        }

        return gameName;
    }

    static string AskForPlayerName()
    {
        var playerName = AnsiConsole.Prompt(new TextPrompt<string>("Enter your name :"));

        if (playerName is null or "") {
            playerName = AskForPlayerName();
        }

        return playerName;
    }

    static string AskForCompanyName()
    {
        var companyName = AnsiConsole.Prompt(new TextPrompt<string>("Enter company name :"));

        if (companyName is null or "") {
            companyName = AskForCompanyName();
        }

        return companyName;
    }

    static int AskForRounds()
    {
        var rounds = AnsiConsole.Prompt(new TextPrompt<int>("How many round ? (15 - 100) :"));

        if (rounds is < 15 or > 100) {
            rounds = AskForRounds();
        }

        return rounds;
    }

    public static (string, string, string, int) Show()
    {
        var gameName = AskForGameName();
        var playerName = AskForPlayerName();
        var companyName = AskForCompanyName();
        var rounds = AskForRounds();

        return (gameName, playerName, companyName, rounds);
    }
}
