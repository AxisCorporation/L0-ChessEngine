using Avalonia.Controls;


using L_0_Chess_Engine.ViewModels;

namespace L_0_Chess_Engine.Views;

public partial class GameView : UserControl
{

    public GameView(int timeSetting)
    {
        InitializeComponent();

        DataContext = new GameViewModel(timeSetting);
    }
}