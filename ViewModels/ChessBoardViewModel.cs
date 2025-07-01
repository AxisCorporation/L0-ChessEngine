using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        for (int row = 7; row >= 0; row--)
        {
            for (int col = 7; col >= 0; col--)
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
            int row = 7 - (i / 8);
            int col = 7 - (i % 8);

            var updatedPiece = Board.Grid[row, col];
            var existingSquare = GridPieces[i];

            existingSquare.Piece = (ChessPiece) updatedPiece;
            existingSquare.UpdateImage();
        }
    }

    
    private void OnSquareClick(SquareViewModel squareClicked)
    {
        if (_selectedSquare is null)
        {
            _selectedSquare = squareClicked;
            _selectedSquare.IsSelected = true;
        }
        else
        {
            Move move = new(_selectedSquare!.Piece, squareClicked.Piece);

            Board.MakeMove(move);

            _selectedSquare.IsSelected = false;
            _selectedSquare = null;
        }
        
    }
}