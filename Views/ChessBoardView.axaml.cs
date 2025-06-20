using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

using L_0_Chess_Engine.Models;
using L_0_Chess_Engine.ViewModels;

namespace L_0_Chess_Engine.Views;

public partial class ChessBoardView : UserControl
{

    private ChessBoardViewModel _viewModel;
    private Grid? _chessBoardGrid;
    private ChessBoard _chessBoard;

    public ChessBoardView()
    {
        InitializeComponent();

        _viewModel = new ChessBoardViewModel();
        _chessBoard = new ChessBoard();

        DataContext = _viewModel;

        Loaded += (s, e) =>
        {
            _chessBoardGrid = this.FindControl<Grid>("ChessBoardGrid");
            if (_chessBoardGrid != null)
            {
                CreateChessBoardUI();
            }
        };

    }

    private void CreateChessBoardUI()
    {
       
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                bool isLightSquare = (row + col) % 2 == 0;
                var background = isLightSquare ? Brushes.Wheat : Brushes.SaddleBrown;
                
                
                var square = new Rectangle
                {
                    Fill = background
                };
                
                // Add to grid
                Grid.SetRow(square, row);
                Grid.SetColumn(square, col);
                _chessBoardGrid.Children.Add(square);
                
                var piece = _chessBoard.Grid[row, col];
                
               // If there's a piece, add it to the board
                if (piece.Type != PieceType.Empty)
                {
                    var pieceColor = (piece.Type & PieceType.White) == PieceType.White ? Brushes.White : Brushes.Black;
                    
                    var textBlock = new TextBlock
                    {
                        Foreground = pieceColor,
                        FontSize = 48,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                    };
                    
                    Grid.SetRow(textBlock, row);
                    Grid.SetColumn(textBlock, col);
                    _chessBoardGrid.Children.Add(textBlock);
                }
            }
        }
    }
}