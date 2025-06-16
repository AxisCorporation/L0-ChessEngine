using Avalonia.Controls;

using L_0_Chess_Engine.Fake;
using L_0_Chess_Engine.ViewModels;

namespace L_0_Chess_Engine.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // ChessBoard myBoard = new ChessBoard();
        // DataContext = new ChessBoardViewModel(myBoard);
    }
}