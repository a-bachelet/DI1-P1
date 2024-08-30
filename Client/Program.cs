using Client.Screens;

using Terminal.Gui;

namespace Client;

public class Program
{
    static void Main()
    {
        Application.Init();
        Application.Invoke(async () => {
            var mainWindow = (MainWindow) Application.Top;

            var titleScreen = new TitleScreen(mainWindow);
            await titleScreen.Show();

            var mainMenuScreen = new MainMenuScreen(mainWindow);
            await mainMenuScreen.Show();

            Application.RequestStop();
        });
        Application.Run<MainWindow>();
        Application.Shutdown();
    }
}

public class MainWindow : Window
{
    public static new string Title => $"Why So Serious ? ({Application.QuitKey} to quit)";

    public MainWindow()
    {
        Width = Dim.Fill();
        Height = Dim.Fill();
        ColorScheme = new ColorScheme(
            new Terminal.Gui.Attribute(Color.Blue, Color.Black),
            new Terminal.Gui.Attribute(Color.Black, Color.White),
            new Terminal.Gui.Attribute(Color.Black, Color.White),
            new Terminal.Gui.Attribute(Color.Blue, Color.Black),
            new Terminal.Gui.Attribute(Color.Black, Color.White)
        );
    }
}
