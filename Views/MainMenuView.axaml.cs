using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia;
using System;
using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.Input;

namespace L_0_Chess_Engine.Views;

public partial class MainMenuView : UserControl
{
    private MainWindow? MainWindow { get; }
    private int TimeSelected { get; set; } = 5; // Default time setting if no options are clicked

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
        var game = new GameView(TimeSelected);
        MainWindow?.SetMainContent(game);
    }

    private void TimeOptionSelected(object? sender, RoutedEventArgs e)
    {
        ToggleButton button = (ToggleButton)sender!;

        TimeSelected = button.Content switch
        {
            "10 min" => 10,
            "15 min" => 15,

            _ => 5 // Default
        };

        foreach (var child in TimeOptions.Children)
        {
            if (child != button)
            {
                ((ToggleButton)child).IsChecked = false;
            }
        }
    }
}