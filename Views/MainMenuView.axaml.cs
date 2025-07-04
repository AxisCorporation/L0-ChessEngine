using Avalonia.Interactivity;
using Avalonia.Controls;

namespace L_0_Chess_Engine.Views;

public partial class MainMenuView : UserControl
{
    private MainWindow? MainWindow { get; }

    public MainMenuView(MainWindow? mainWindow)
    {
        InitializeComponent();
        MainWindow = mainWindow;

        PlayToggle.Checked += (_, _) =>
        {
            PlayToggle.IsVisible = false; // Play itself gone
            
            GameModeOptions.IsVisible = true;
            GameModeOptions.Opacity = 1;
            
            // show Local time options
            // TimeOptions.IsVisible = true;
            // TimeOptions.Opacity = 1;

            OtherOptions.IsVisible = false; // hide credits/quit
        };

        LocalGame.Click += PlayGame;

        AiGame.Click += PlayGame;
    }

    public void PlayGame(object? sender, RoutedEventArgs e)
    {
        var game = new GameView();
        MainWindow?.SetMainContent(game);
    }
}