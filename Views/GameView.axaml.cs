using Avalonia.Controls;
using L_0_Chess_Engine.Enums;
using L_0_Chess_Engine.ViewModels;

namespace L_0_Chess_Engine.Views;

public partial class GameView : UserControl
{
    public GameView(int timeSetting, MainWindow mainWindow, bool AiGame = false, AIDifficulty Difficulty = AIDifficulty.Easy)
    {
        InitializeComponent();

        DataContext = new GameViewModel(timeSetting, AiGame, Difficulty, mainWindow);

        (DataContext as GameViewModel)!.MovesCN.CollectionChanged += (_, _) => MovesScroller.ScrollToEnd();
    }
}