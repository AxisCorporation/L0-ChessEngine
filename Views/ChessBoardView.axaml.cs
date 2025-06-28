using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Controls.Shapes;

using L_0_Chess_Engine.ViewModels;

namespace L_0_Chess_Engine.Views;

public partial class ChessBoardView : UserControl
{
    private ChessBoardViewModel _chessBoardVM;
    private Grid? _chessBoardGrid;

    private (int row, int col)? _selectedSquare = null;


    public ChessBoardView()
    {
        InitializeComponent();

        _chessBoardVM = new ChessBoardViewModel();

        DataContext = _chessBoardVM;

        
    }
}