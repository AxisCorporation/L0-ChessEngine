using System.Threading.Tasks;
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

        GameOverButton.Click += async (s, e) => await GoToMainMenu(s, e);
        MainMenuButton.Click += async (s, e) => await GoToMainMenu(s, e);

        (DataContext as GameViewModel)!.MovesCN.CollectionChanged += (_, _) => MovesScroller.ScrollToEnd();
    }

    private async Task GoToMainMenu(object? Sender, RoutedEventArgs e)
    {
        await Task.Run(() => SoundPlayer.Play("Assets/Audio/ButtonClick.mp3"));
        ChessBoard.Instance.ResetBoard();
        mainWindow.SetMainContent(new MainMenuView(mainWindow));
    }
}