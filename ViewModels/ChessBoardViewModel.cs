using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using L_0_Chess_Engine.Contracts;
using L_0_Chess_Engine.Models;
using L_0_Chess_Engine.SampleModels;

namespace L_0_Chess_Engine.ViewModels;

public partial class ChessBoardViewModel : ObservableObject
{
    public ObservableCollection<SquareViewModel> GridPieces { get; set; } = [];
    private ChessBoard Board { get; set; } = new();
    
    private SquareViewModel? _selectedSquare;

    public ChessBoardViewModel()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var piece = Board.Grid[row, col]; // or however your grid is structured
                
                SquareViewModel square = new((ChessPiece) piece)
                {
                    IsLightSquare = (row + col) % 2 == 0
                };

                square.ClickCommand = new RelayCommand(() => OnSquareClick(square));

                GridPieces.Add(square);
            }
        }

        Board.GridUpdated += UpdateGrid;
    }

    private void UpdateGrid()
    {
        for (int i = 0; i < 64; i++)
        {
            int row = i / 8;
            int col = i % 8;

            var updatedPiece = Board.Grid[row, col];
            var existingSquare = GridPieces[i];

            existingSquare.Piece = (ChessPiece) updatedPiece;
            existingSquare.UpdateImage();
        }
    }

    
    private void OnSquareClick(SquareViewModel squareClicked)
    {
        if (_selectedSquare is null && squareClicked.Piece.Type != PieceType.Empty)
        {
            _selectedSquare = squareClicked;
            _selectedSquare.IsSelected = true;
        }
        else
        {
            if (squareClicked != _selectedSquare)
            {
                MovePiece(_selectedSquare.Piece.Coordinates.Y - 1, _selectedSquare.Piece.Coordinates.X - 1,
                    squareClicked.Piece.Coordinates.Y - 1, squareClicked.Piece.Coordinates.X - 1);
                
                // How I imagine the function will Work after all is well and done
                // Move move = new Move(_selectedSquare.Piece, squareClicked.Piece);
                // Board.MakeMove(thisMove);
            }
            _selectedSquare.IsSelected = false;
            _selectedSquare = null;
        }
        
    }

    // This Function is for testing purposes anyways, delete it later anyways
    public void MovePiece(int fromRow, int fromCol, int toRow, int toCol)
    {
        if (fromRow == toRow && fromCol == toCol)
        {
            return;
        }
        
        Board.Grid[toCol, toRow].Type = Board.Grid[fromCol, fromRow].Type;
        Board.Grid[fromCol, fromRow].Type = PieceType.Empty;
        
        UpdateGrid();
        
        // GridPieces[toRow * 8 + toCol].Piece = GridPieces[fromRow * 8 + fromCol].Piece;
        // Coordinate corr = new Coordinate(fromCol, fromRow);
        // GridPieces[fromRow * 8 + fromCol].Piece = new ChessPiece(PieceType.Empty, corr);

    }


    // For testing
    [RelayCommand]
    public void ClickMe()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                GridPieces[i * 8 + j] = new SquareViewModel(PieceType.Rook | PieceType.White, new(i + 1, j + 1));
            }
        }
        
    }
}