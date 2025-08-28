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

        GameOverButton.Click += async (_, _) => await GoToMainMenu();
        MainMenuButton.Click += async (_, _) => await GoToMainMenu();

        (DataContext as GameViewModel)!.MovesCN.CollectionChanged += (_, _) => MovesScroller.ScrollToEnd();
    }

    private async Task GoToMainMenu()
    {
        await SoundPlayer.Play(SoundPlayer.ClickSFXPath);
        ChessBoard.Instance.ResetBoard();
        mainWindow.SetMainContent(new MainMenuView(mainWindow));
    }
}