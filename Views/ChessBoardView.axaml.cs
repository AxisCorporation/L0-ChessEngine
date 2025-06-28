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

        Loaded += (s, e) =>
        {
            _chessBoardGrid = this.FindControl<Grid>("ChessBoardGrid");
            CreateChessBoardUI();
        };

        _chessBoardVM.BoardChanged += CreateChessBoardUI;
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
                    Background = background,
                    Tag = (row, col) // Store position
                };

                var Piece = _chessBoardVM.GridPieces[row * 8 + col];
                var Image = new Image
                {
                    Source = Piece.Image
                };

                square.PointerPressed += OnSquareClicked;

                square.Children.Add(Image);

                // Add to grid
                Grid.SetRow(square, row);
                Grid.SetColumn(square, col);

                _chessBoardGrid.Children.Add(square);


                // Grid.SetRow(Image, row);
                // Grid.SetColumn(Image, col);
                // _chessBoardGrid.Children.Add(Image);
            }
        }
    }

    private void OnSquareClicked(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Grid square && square.Tag is ValueTuple<int, int> position)
        {
            HighlightSquare(square);
            
            var (row, col) = position;

            if (_selectedSquare == null)
            {
                // First click - select source square
                if (_chessBoardVM.GridPieces[row * 8 + col].Image != null)
                {
                    _selectedSquare = (row, col);
                    // HighlightSquare(row, col);
                }
            }
            else
            {
                var (fromRow, fromCol) = _selectedSquare.Value;

                // Perform move
                _chessBoardVM.MovePiece(fromRow, fromCol, row, col);
                _selectedSquare = null;

                // Redraw board
                CreateChessBoardUI();
            }
        }
    }

    private void HighlightSquare(Grid square)
    {
        var overlay = new Rectangle
        {
            Fill = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0)), // light red overlay
            IsHitTestVisible = false // so it doesn’t block clicks
        };

        square.Children.Add(overlay); // if square is a Grid

    }
}