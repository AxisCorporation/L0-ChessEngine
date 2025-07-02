using Avalonia.Controls;
using Avalonia.Interactivity;

namespace L_0_Chess_Engine.Views;

public partial class MainMenuView : UserControl
{
    public MainWindow? MainWindow { get; set; }
    
    public MainMenuView(MainWindow? mainWindow)
    {
        InitializeComponent();
        MainWindow = mainWindow;
    }

    public void PlayGame(object? sender, RoutedEventArgs e)
    {
        var chessBoard = new GameView();
        MainWindow?.SetMainContent(chessBoard);
    }
}