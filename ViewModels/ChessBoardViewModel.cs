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
    public ObservableCollection<ChessPieceViewModel> GridPieces { get; set; } = [];
    private ChessBoard Board { get; set; } = new ChessBoard();
    public event Action? BoardChanged;

    public ChessBoardViewModel()
    {
        foreach (var Piece in Board.Grid)
        {
            GridPieces.Add(new ChessPieceViewModel((ChessPiece)Piece));
        }
        Board.GridUpdated += UpdateGrid;
    }

    public void UpdateGrid()
    {
        GridPieces = [];
        foreach (var Piece in Board.Grid)
        {
            GridPieces.Add(new ChessPieceViewModel((ChessPiece)Piece));
        }
    }


    public void MovePiece(int fromRow, int fromCol, int toRow, int toCol)
    {
        if (fromRow == toRow && fromCol == toCol)
        {
            return;
        }
        
        GridPieces[toRow * 8 + toCol] = GridPieces[fromRow * 8 + fromCol];

        GridPieces[fromRow * 8 + fromCol] = new ChessPieceViewModel(PieceType.Empty, new(fromRow, fromCol));

    }


    // For testing
    [RelayCommand]
    public void ClickMe()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                GridPieces[i * 8 + j] = new ChessPieceViewModel(PieceType.Rook | PieceType.White, new(i + 1, j + 1));
            }
        }

        BoardChanged?.Invoke();
    }
}