using Avalonia.Controls;


using L_0_Chess_Engine.ViewModels;

namespace L_0_Chess_Engine.Views;

public partial class GameView : UserControl
{
    public GameView(int timeSetting, bool AiGame = false)
    {
        InitializeComponent();

        if (AiGame)
        {
            DataContext = new GameViewModel(timeSetting, true);
        }
        else DataContext = new GameViewModel(timeSetting);
    }
}