using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia;
using System;
using Avalonia.Controls.Primitives;
using System.Collections.Generic;
using L_0_Chess_Engine.Enums;

namespace L_0_Chess_Engine.Views;

public partial class MainMenuView : UserControl
{
    private int TimeSelected { get; set; } = 5; // Default time setting if no options are clicked
    private AIDifficulty DifficultySelected { get; set; } 

    private bool _isNavigatingBack = false;
    Stack<StackPanel> _panelHistory = [];
    private MainWindow? MainWindow { get; }

    private StackPanel? _currentPanel;
    private StackPanel? CurrentPanel
    {
        set
        {
            if (_currentPanel is not null)
            {
                if (!_isNavigatingBack)
                {
                    _panelHistory.Push(_currentPanel);
                }
                _currentPanel.IsVisible = false;
            }

            _currentPanel = value;

            bool isMain = _currentPanel == MainMenuOptions;

            if (isMain)
            {
                _panelHistory.Clear();
            }

            BackButton.IsVisible = !isMain;
            TimeOptionsText.IsVisible = !isMain;
            TimeOptions.IsVisible = !isMain;

            _isNavigatingBack = false;
            _currentPanel!.IsVisible = true;
        }
    }

    public MainMenuView(MainWindow? mainWindow)
    {
        InitializeComponent();
        MainWindow = mainWindow;

        CurrentPanel = MainMenuOptions;

        PlayToggle.Click += (_, _) => CurrentPanel = GameModeOptions;
        AiGame.Click += (_, _) => CurrentPanel = DifficultyOptions;
        QuitButton.Click += (_, _) => Environment.Exit(0);

        LocalGame.Click += PlayGame;

        BackButton.Click += (_, _) =>
        {
            _isNavigatingBack = true;
            if (_panelHistory.Count != 0)
            {
                CurrentPanel = _panelHistory.Pop();
            }
        };
    }

    
    private void TimeOptionSelected(object? sender, RoutedEventArgs e)
    {
        ToggleButton button = (ToggleButton) sender!;

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
    
    private void PlayGame(object? sender, RoutedEventArgs e) => MainWindow?.SetMainContent(new GameView(TimeSelected));

    private void PlayAIGame(object? sender, RoutedEventArgs e)
    {
        Button button = (Button)sender!;

        DifficultySelected = button.Content switch
        {
            "Medium" => AIDifficulty.Medium,
            "Hard" => AIDifficulty.Hard,

            _ => AIDifficulty.Easy
        };

        GameView game = new(TimeSelected, AiGame: true, Difficulty: DifficultySelected);

        MainWindow?.SetMainContent(game);
    }
}