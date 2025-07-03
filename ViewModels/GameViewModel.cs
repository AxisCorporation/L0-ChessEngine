using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using L_0_Chess_Engine.Models;

namespace L_0_Chess_Engine.ViewModels;

public partial class GameViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _turnText;

    private ChessBoard Board { get; set; } = ChessBoard.Instance;

    public ObservableCollection<SquareViewModel> GridPieces { get; set; } = [];
    public bool IsWhiteTurn { get; set; }
    private SquareViewModel? _selectedSquare;

    public GameViewModel()
    {
        IsWhiteTurn = true;
        UpdateTurnString();

        for (int row = 0; row < 8; row++)
        {
            int y = 7 - row;
            for (int col = 0; col < 8; col++)
            {
                var piece = Board.Grid[y, col]; // or however your grid is structured

                SquareViewModel square = new((ChessPiece)piece)
                {
                    IsLightSquare = (row + col) % 2 == 0
                };

                square.ClickCommand = new RelayCommand(() => OnSquareClick(square),
                                                       () => CanClickSquare(square));

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
            int col = i % 8;

            var updatedPiece = Board.Grid[row, col];
            var existingSquare = GridPieces[i];

            existingSquare.Piece = (ChessPiece)updatedPiece;
            existingSquare.UpdateImage();
        }
    }


    private void OnSquareClick(SquareViewModel squareClicked)
    {
        Console.WriteLine($"{squareClicked.Piece.Type} | {squareClicked.Piece.Coordinates}");

        if (_selectedSquare is null)
        {
            _selectedSquare = squareClicked;
            _selectedSquare.IsSelected = true;
        }
        else
        {
            Move move = new(_selectedSquare!.Piece, squareClicked.Piece);

            if (!move.IsValid)
            {
                _selectedSquare.IsSelected = false;
                _selectedSquare = null;
                return;
            }

            Board.MakeMove(move);

            IsWhiteTurn = !IsWhiteTurn;

            _selectedSquare.IsSelected = false;
            _selectedSquare = null;
        }
        
        NotifyCanClickSquare();
        UpdateTurnString();
    }

    private void NotifyCanClickSquare()
    {
        foreach (var piece in GridPieces)
        {
            ((RelayCommand)piece.ClickCommand!).NotifyCanExecuteChanged();
        }
    }

    private bool CanClickSquare(SquareViewModel squareClicked)
    {
        if (_selectedSquare is null && squareClicked.Piece != PieceType.Empty && squareClicked.Piece.IsWhite == IsWhiteTurn)
        {
            return true;
        }
        else if (_selectedSquare is not null)
        {
            return true;
        }

        return false;
    }


    private void UpdateTurnString() => TurnText = IsWhiteTurn ? "White's turn!" : "Black's turn!";
    
}