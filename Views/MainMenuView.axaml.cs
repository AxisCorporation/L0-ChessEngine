using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia;
using System;
using Avalonia.Controls.Primitives;
using System.Collections.Generic;
using L_0_Chess_Engine.Enums;
using System.Diagnostics;

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
            // There needs to be a way to check if the panel is part of the "main" menu for better scaling, 
            // but for now this works 
            TimeOptions.IsVisible = !isMain && _currentPanel != CreditsPanel;

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
        CreditsButton.Click += (_, _) => CurrentPanel = CreditsPanel;
        AiGame.Click += (_, _) => CurrentPanel = DifficultyOptions;
        QuitButton.Click += (_, _) => Environment.Exit(0);

        LocalGame.Click += PlayGame;

        BackButton.Click += (_, _) =>
        {
            _isNavigatingBack = true;
            CurrentPanel = _panelHistory.Pop();
        };
    }


    private void TimeOptionSelected(object? sender, RoutedEventArgs e)
    {
        ToggleButton buttonSelected = (ToggleButton)sender!;
        buttonSelected.IsChecked = true;

        TimeSelected = buttonSelected.Content switch
        {
            "10 min" => 10,
            "15 min" => 15,

            _ => 5 // Default
        };

        foreach (var child in TimeOptions.Children)
        {
            if (child is StackPanel TimeOptionsPanel)
            {
                foreach (var button in TimeOptionsPanel.Children)
                {
                    if (button != buttonSelected)
                    {
                        ((ToggleButton)button).IsChecked = false;
                    }
                }
                break;
            }
        }
    }

    private void PlayGame(object? sender, RoutedEventArgs e) => MainWindow?.SetMainContent(new GameView(TimeSelected, MainWindow));

    private void PlayAIGame(object? sender, RoutedEventArgs e)
    {
        Button button = (Button)sender!;

        DifficultySelected = button.Content switch
        {
            "Medium" => AIDifficulty.Medium,
            "Hard" => AIDifficulty.Hard,

            _ => AIDifficulty.Easy
        };

        GameView game = new(TimeSelected, MainWindow, AiGame: true, Difficulty: DifficultySelected);

        MainWindow?.SetMainContent(game);
    }
}