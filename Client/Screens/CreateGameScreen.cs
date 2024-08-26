using System.Net.Http.Json;
using System.Text.Json;

using Client.Prompts;

using Spectre.Console;

namespace Client.Screens;

public static class CreateGameScreen
{
    public static async Task<bool> Show()
    {
        AnsiConsole.Clear();

        var (gameName, playerName, companyName, rounds) = CreateGamePrompt.Show();

        var httpClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true }) {
            BaseAddress = new Uri("https://localhost:7032")
        };
        var requestBody = new {gameName, playerName, companyName, rounds};
        var request = httpClient.PostAsJsonAsync("/games", requestBody);
        var response = await request;

        if (!response.IsSuccessStatusCode)
        {
            return true;
        }


        try {
            var content = await response.Content.ReadFromJsonAsync<JsonElement>();
            var gameId = content.GetProperty("id").GetInt32();

            return await new CurrentGameScreen(gameId, playerName).Show();
        }
        catch(Exception e) {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Markup("[red]Error: An error occured[/]\n"));
            AnsiConsole.WriteException(e);

            return true;
        }
    }
}
