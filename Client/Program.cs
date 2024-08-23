using Client.Screens;

namespace Client;

public class Program
{
    static async Task Main()
    {
        var exit = false;

        while (!exit)
        {
            await TitleScreen.Show();
            exit = await MainMenuScreen.Show();
        }
    }
}
