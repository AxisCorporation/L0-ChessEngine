using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia;
using System;

namespace L_0_Chess_Engine.Views;

public partial class MainMenuView : UserControl
{
    private MainWindow? MainWindow { get; }

    public MainMenuView(MainWindow? mainWindow)
    {
        InitializeComponent();
        MainWindow = mainWindow;

        PlayToggle.Click += (_, _) =>
        {
            PlayToggle.IsVisible = false; // Play itself gone
            
            GameModeOptions.IsVisible = true;

            BackButton.IsVisible = true;

            OtherOptions.IsVisible = false; // hide credits/quit
        };

        BackButton.Click += (_, _) =>
        {
            PlayToggle.IsVisible = true;

            GameModeOptions.IsVisible = false;

            BackButton.IsVisible = false;

            OtherOptions.IsVisible = true; // hide credits/quit
        };

        QuitButton.Click += (_, _) => Environment.Exit(0);

        LocalGame.Click += PlayGame;

        AiGame.Click += PlayGame;
    }

    public void PlayGame(object? sender, RoutedEventArgs e)
    {
        var game = new GameView();
        MainWindow?.SetMainContent(game);
    }
}