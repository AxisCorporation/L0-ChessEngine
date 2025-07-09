using System.Diagnostics;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using L_0_Chess_Engine.Models;
using System;
using System.Timers;

namespace L_0_Chess_Engine.ViewModels;

public partial class GameViewModel : ObservableObject
{
    public TimeSpan WhiteTime { get; set; }

    [ObservableProperty]
    private string _whiteTimeText;

    public TimeSpan BlackTime { get; set; }

    [ObservableProperty]
    private string _blackTimeText;

    [ObservableProperty]
    private string? _turnText;

    // This is a Temp Variable
    [ObservableProperty]
    private string? _checkText = "";

    private ChessBoard Board { get; set; } = ChessBoard.Instance;

    public ObservableCollection<SquareViewModel> GridPieces { get; set; } = [];

    private bool _isWhiteTurn;
    public bool IsWhiteTurn
    {
        get => _isWhiteTurn;
        set
        {
            _isWhiteTurn = value;
            NotifyCanClickSquare();
            UpdateTurnString();
        }
    }
    private SquareViewModel? _selectedSquare;

    public GameViewModel(int timeLimit)
    {
        IsWhiteTurn = true;

        WhiteTime = TimeSpan.FromMinutes(timeLimit);
        BlackTime = TimeSpan.FromMinutes(timeLimit);

        Timer timer = new(100)
        {
            Enabled = true
        }; // ticks every 100 milliseconds

        timer.Elapsed += (_, _) =>
        {
            if (IsWhiteTurn)
            {
                WhiteTime = WhiteTime.Subtract(TimeSpan.FromMilliseconds(100));
            }
            else
            {
                BlackTime = BlackTime.Subtract(TimeSpan.FromMilliseconds(100));
            }
        };

        for (int row = 7; row >= 0; row--)
        {
            for (int col = 0; col < 8; col++)
            {
                var piece = Board.Grid[col, row]; // or however your grid is structured

                SquareViewModel square = new(piece)
                {
                    IsLightSquare = (row + col + 1) % 2 == 0
                };

                square.ClickCommand = new RelayCommand(() => OnSquareClick(square), () => CanClickSquare(square));

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
            int col = 7 - (i % 8);

            var updatedPiece = Board.Grid[col, row];
            var existingSquare = GridPieces[63 - i];

            existingSquare.Piece = updatedPiece;
            existingSquare.UpdateImage();
        }
    }


    private void OnSquareClick(SquareViewModel squareClicked)
    {
        Debug.WriteLine($"DEBUG: {squareClicked.Piece.Type} | {squareClicked.Piece.Coordinates}");

        if (_selectedSquare is null)
        {
            _selectedSquare = squareClicked;
            _selectedSquare.IsSelected = true;
            NotifyCanClickSquare();
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

        UpdateCheckString();
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

    private void UpdateCheckString()
    {
        if (IsWhiteTurn && Board.IsCheck)
        {
            CheckText = "White is in Check";
        }
        else if (!IsWhiteTurn && Board.IsCheck)
        {
            CheckText = "Black is in Check";
        }
        else CheckText = "";
    }

    private void StartTimer()
    {
    }
}