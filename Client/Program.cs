using Client.Screens;

using Terminal.Gui;

namespace Client;

public class Program
{
    static void Main()
    {
        Application.Init();
        Colors.Base.Normal = Application.Driver.MakeAttribute(Color.Blue, Color.Black);
        Application.MainLoop.Invoke(async () => {
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
    }
}
