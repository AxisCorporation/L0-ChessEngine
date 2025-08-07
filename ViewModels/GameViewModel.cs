using System.Diagnostics;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using L_0_Chess_Engine.Models;
using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using L_0_Chess_Engine.Views;
using L_0_Chess_Engine.AI;

namespace L_0_Chess_Engine.ViewModels;

public partial class GameViewModel : ObservableObject
{
    public bool GameRunning { get; set; }
    public TimeSpan WhiteTimer { get; set; }

    [ObservableProperty]
    private string _whiteTimerText;

    public TimeSpan BlackTimer { get; set; } // Why is this Public?

    [ObservableProperty]
    private string _blackTimerText;

    [ObservableProperty]
    private string? _turnText;

    private readonly string _timeFormat = @"m\:ss";

    // This is a Temp Variable
    [ObservableProperty]
    private string? _gameStateText = "";

    private ChessBoard Board { get; set; } = ChessBoard.Instance;

    private Ai _ai;
    private Window? _promotionWindow;

    public ObservableCollection<SquareViewModel> GridPieces { get; set; } = [];

    private bool _isWhiteTurn;
    public bool IsWhiteTurn
    {
        get => _isWhiteTurn;
        set
        {
            _isWhiteTurn = value;
            NotifyCanClickSquares();
            UpdateTurnText();
        }
    }

    private SquareViewModel? _selectedSquare;

    public GameViewModel(int timeLimit, bool LoadAi = false)
    {
        WhiteTimer = TimeSpan.FromMinutes(timeLimit);
        BlackTimer = TimeSpan.FromMinutes(timeLimit);

        WhiteTimerText = WhiteTimer.ToString(_timeFormat); // Formats the time in 5:00
        BlackTimerText = BlackTimer.ToString(_timeFormat);

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
        GameRunning = true;
        IsWhiteTurn = true;

        if (LoadAi)
        {
            LoadAiModule();
        }

        _ = UpdateTurnTimersAsync();
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
            NotifyCanClickSquares();
        }
        else
        {
            HandleMove(squareClicked);
        }

    }

    private void HandleMove(SquareViewModel squareClicked)
    {
        Move move = new(_selectedSquare!.Piece, squareClicked.Piece);

        if (!move.IsValid || move.TargetsKing || ChessBoard.Instance.WouldCauseCheck(move))
        {
            _selectedSquare.IsSelected = false;
            _selectedSquare = null;

            GameStateText = move.ErrorMessage != string.Empty ? move.ErrorMessage : "Invalid move!";
            return;
        }


        if (IsPawnPromotionMove(_selectedSquare.Piece, squareClicked.Piece))
        {
            ShowPawnPromotionDialog(_selectedSquare.Piece.IsWhite, () => RegisterMove(move));
            return;
        }
        else
        {
            RegisterMove(move);
        }

        if (_ai is not null)
        {
            MakeAiMove();
        }
    }

    private void RegisterMove(Move move)
    {
        Board.MakeMove(move);

        _selectedSquare!.IsSelected = false;
        _selectedSquare = null;

        IsWhiteTurn = !IsWhiteTurn;

        UpdateGameStateText(move);
    }

    private static bool IsPawnPromotionMove(ChessPiece initPiece, ChessPiece destPiece)
    {
        // Check if the piece is a pawn
        if (!initPiece.EqualsUncolored(PieceType.Pawn))
        {
            return false;
        }

        return destPiece.Coordinates.Y == 7 || destPiece.Coordinates.Y == 0;
    }

    private void ShowPawnPromotionDialog(bool isWhite, Action onPieceSelected)
    {
        var promotionView = new PawnPromotionView(isWhite);
        var viewModel = (PawnPromotionViewModel)promotionView.DataContext!;

        viewModel.PieceSelected += () =>
        {
            _promotionWindow!.Close();
            onPieceSelected?.Invoke();
        };

        var window = new Window
        {
            Content = promotionView,
            Width = 300,
            Height = 150,
            CanResize = false,
            ShowInTaskbar = false,
            Title = "Pawn Promotion"
        };

        _promotionWindow = window;

        window.Show();
    }


    private void NotifyCanClickSquares()
    {
        foreach (var piece in GridPieces)
        {
            ((RelayCommand)piece.ClickCommand!).NotifyCanExecuteChanged();
        }
    }

    private bool CanClickSquare(SquareViewModel squareClicked)
    {
        if (!GameRunning)
        {
            return false;
        }

        if (_ai is not null && !IsWhiteTurn)
        {
            return false;
        }

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

    // This Function will probably be redundant later on
    private void MakeAiMove()
    {
        Move move = _ai.GenerateMove();

        Board.MakeMove(move);
        IsWhiteTurn = !IsWhiteTurn;

        UpdateGameStateText(move);
    }

    private void LoadAiModule() => _ai = new Ai(true);

    private void UpdateTurnText() => TurnText = IsWhiteTurn ? "White's turn!" : "Black's turn!";

    private void UpdateGameStateText(Move? move = null)
    {
        if (Board.IsCheckMate)
        {
            GameStateText = AppendMove(IsWhiteTurn ? "White is in Checkmate!" : "Black is in Checkmate!", move);
        }
        else if (Board.IsCheck)
        {
            GameStateText = AppendMove(IsWhiteTurn ? "White is in Check!" : "Black is in Check!", move);
        }
        else if (WhiteTimer <= TimeSpan.Zero)
        {
            GameStateText = "Time's up for White - Black wins!";
        }
        else if (BlackTimer <= TimeSpan.Zero)
        {
            GameStateText = "Time's up for Black - White wins!";
        }
        else
        {
            GameStateText = AppendMove("", move);
        }
    }

    private async Task UpdateTurnTimersAsync()
    {
        while (GameRunning)
        {
            await Task.Delay(100);
            if (IsWhiteTurn)
            {
                WhiteTimer -= TimeSpan.FromMilliseconds(100);
                WhiteTimerText = WhiteTimer.ToString(_timeFormat);
            }
            else
            {
                BlackTimer -= TimeSpan.FromMilliseconds(100);
                BlackTimerText = BlackTimer.ToString(_timeFormat);
            }

            if (WhiteTimer <= TimeSpan.Zero || BlackTimer <= TimeSpan.Zero)
            {
                GameRunning = false;

                NotifyCanClickSquares();
                UpdateGameStateText();
            }
        }
    }

    private static string AppendMove(string Message, Move? move)
    {
        if (!string.IsNullOrWhiteSpace(Message))
        {
            Message = Message.Insert(0, "- ");
        }        

        return move is not null ? $"{move.GeneratePieceMovedMessage()} {Message}" : Message; 
    }
}