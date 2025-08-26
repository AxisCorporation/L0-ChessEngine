using Avalonia.Controls;
using Avalonia.Interactivity;
using L_0_Chess_Engine.Common;
using L_0_Chess_Engine.Enums;
using L_0_Chess_Engine.Models;
using L_0_Chess_Engine.ViewModels;

namespace L_0_Chess_Engine.Views;

public partial class GameView : UserControl
{
    private MainWindow mainWindow;

    public GameView(int timeSetting, MainWindow mainMenu, bool AiGame = false, AIDifficulty Difficulty = AIDifficulty.Easy)
    {
        mainWindow = mainMenu; // this variable naming is confusing as hell
        InitializeComponent();

        DataContext = new GameViewModel(timeSetting, AiGame, Difficulty);

        GameOverButton.Click += GoToMainMenu;
        MainMenuButton.Click += GoToMainMenu;

        (DataContext as GameViewModel)!.MovesCN.CollectionChanged += (_, _) => MovesScroller.ScrollToEnd();
    }

    private void GoToMainMenu(object? Sender, RoutedEventArgs e)
    {
        SoundPlayer.Play("Assets/Audio/ButtonClick.mp3");
        ChessBoard.Instance.ResetBoard();
        mainWindow.SetMainContent(new MainMenuView(mainWindow));
    }
}