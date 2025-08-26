using Avalonia.Interactivity;
using Avalonia.Controls;
using System;
using Avalonia.Controls.Primitives;
using System.Collections.Generic;
using L_0_Chess_Engine.Enums;
using L_0_Chess_Engine.Common;

namespace L_0_Chess_Engine.Views;

public partial class MainMenuView : UserControl
{
    private int TimeSelected { get; set; } = 10; // Default time setting if no options are clicked
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
        
        PlayToggle.Click += (_, _) =>
        {
            SoundPlayer.Play("Assets/Audio/ButtonClick.mp3");
            CurrentPanel = GameModeOptions;
        };
        CreditsButton.Click += (_, _) =>
        {
            SoundPlayer.Play("Assets/Audio/ButtonClick.mp3");
            CurrentPanel = CreditsPanel;
        };
        AiGame.Click += (_, _) =>
        {
            SoundPlayer.Play("Assets/Audio/ButtonClick.mp3");
            CurrentPanel = DifficultyOptions;
        };
        
        QuitButton.Click += (_, _) =>
        {
            SoundPlayer.Play("Assets/Audio/ButtonClick.mp3");
            Environment.Exit(0);
        };

        LocalGame.Click += PlayGame;

        BackButton.Click += (_, _) =>
        {
            SoundPlayer.Play("Assets/Audio/ButtonClick.mp3");
            _isNavigatingBack = true;
            CurrentPanel = _panelHistory.Pop();
        };
    }


    private void TimeOptionSelected(object? sender, RoutedEventArgs e)
    {
        SoundPlayer.Play("Assets/Audio/ButtonClick.mp3");
        ToggleButton buttonSelected = (ToggleButton)sender!;
        buttonSelected.IsChecked = true;

        TimeSelected = buttonSelected.Content switch
        {
            "5 min" => 5,
            "10 min" => 10,
            "15 min" => 15,

            _ => 10 // Default
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

    private void PlayGame(object? sender, RoutedEventArgs e)
    {
        SoundPlayer.Play("Assets/Audio/ButtonClick.mp3");
        MainWindow?.SetMainContent(new GameView(TimeSelected, MainWindow));
    }

    private void PlayAIGame(object? sender, RoutedEventArgs e)
    {
        SoundPlayer.Play("Assets/Audio/ButtonClick.mp3");
        
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