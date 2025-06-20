using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
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
                
                
                var square = new Grid
                {
                    Background = background
                };
                
                // Add to grid
                Grid.SetRow(square, row);
                Grid.SetColumn(square, col);
                _chessBoardGrid.Children.Add(square);

                var Piece = new ChessPieceViewModel((ChessPiece) _chessBoard.Grid[row, col]);
                var Image = new Image()
                {
                    Source = Piece.Image
                };

                Grid.SetRow(Image, row);
                Grid.SetColumn(Image, col);
                _chessBoardGrid.Children.Add(Image);
            }
        }
    }
}