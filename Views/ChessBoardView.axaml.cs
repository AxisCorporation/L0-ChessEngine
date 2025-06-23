using Avalonia.Controls;
using Avalonia.Media;
using L_0_Chess_Engine.ViewModels;

namespace L_0_Chess_Engine.Views;

public partial class ChessBoardView : UserControl
{

    private ChessBoardViewModel _chessBoardVM;
    private Grid? _chessBoardGrid;

    public ChessBoardView()
    {
        InitializeComponent();

        _chessBoardVM = new ChessBoardViewModel();

        DataContext = _chessBoardVM;

        Loaded += (s, e) =>
        {
            _chessBoardGrid = this.FindControl<Grid>("ChessBoardGrid");
            CreateChessBoardUI();
        };

        _chessBoardVM.OnBoardChanged += CreateChessBoardUI;

    }

    private void CreateChessBoardUI()
    {
        if (_chessBoardGrid is null)
        {
            return;
        }
        
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

                var Piece = _chessBoardVM.GridPieces[row * 8 + col];
                var Image = new Image
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