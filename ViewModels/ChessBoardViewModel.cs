using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using L_0_Chess_Engine.Models;
using L_0_Chess_Engine.SampleModels;

namespace L_0_Chess_Engine.ViewModels;

public partial class ChessBoardViewModel : ObservableObject
{
    public ObservableCollection<SquareViewModel> GridPieces { get; set; } = [];
    private ChessBoard Board { get; set; } = new();
    public event Action? BoardChanged;

    public ChessBoardViewModel()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var piece = Board.Grid[row, col]; // or however your grid is structured

                GridPieces.Add(new SquareViewModel((ChessPiece)piece)
                {
                    IsLightSquare = (row + col) % 2 == 0
                });
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


    public void MovePiece(int fromRow, int fromCol, int toRow, int toCol)
    {
        if (fromRow == toRow && fromCol == toCol)
        {
            return;
        }
        
        GridPieces[toRow * 8 + toCol] = GridPieces[fromRow * 8 + fromCol];

        GridPieces[fromRow * 8 + fromCol] = new SquareViewModel(PieceType.Empty, new(fromRow, fromCol));

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

        BoardChanged?.Invoke();
    }
}