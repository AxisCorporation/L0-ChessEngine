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
using L_0_Chess_Engine.Enums;

namespace L_0_Chess_Engine.ViewModels;

public partial class GameViewModel : ObservableObject
{
    public bool GameRunning { get; set; }
    public TimeSpan WhiteTimer { get; set; }

    [ObservableProperty]
    private string _whiteTimerText;

    private TimeSpan BlackTimer { get; set; } // Why is this Public?

    [ObservableProperty]
    private string _blackTimerText;

    [ObservableProperty]
    private string? _turnText;

    private readonly string _timeFormat = @"m\:ss";

    // This is a Temp Variable
    [ObservableProperty]
    private string? _gameStateText = "";

    private ChessBoard Board { get; set; } = ChessBoard.Instance;

    private Move? _playerMove = null;

    private bool _aiGame;
    private bool _lockInput = false;

    private bool LockInput
    {
        get => _lockInput;
        set
        {
            _lockInput = value;
            NotifyCanClickSquares();
        }
    }
    private Ai? _ai = null;
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

    public GameViewModel(int timeLimit, bool LoadAi, AIDifficulty Difficulty = AIDifficulty.Easy)
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
            LoadAiModule(Difficulty);
            _aiGame = true;
        }

        // Don't like this syntax
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

    private async Task ManageTurns()
    {
        if (_playerMove is not null)
        {
            // Resetting Selection
            if (_selectedSquare is not null)
            {
                _selectedSquare.IsSelected = false;
            }

            _selectedSquare = null;

            if (!RegisterMove(_playerMove))
            {
                Debug.WriteLine("Invalid player move, resetting selection.");
                return;
            }
        }

        if (_aiGame && !IsWhiteTurn)
        {
            LockInput = true;

            Move aiMove = await Task.Run(() => _ai.GenerateMove());


            LockInput = false;
            if (!RegisterMove(aiMove))
            {
                Debug.WriteLine("AI attempted invalid move! Skipping turn.");
                IsWhiteTurn = !IsWhiteTurn;
                GameStateText = "AI Made Invalid Move!";
                return;
            }

            // Loop Again?
            // await ManageTurns();
        }
        
        if (!GameRunning)
        {
            EndGame();
        }
        
    }


    private async Task OnSquareClick(SquareViewModel squareClicked)
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
            _playerMove = new(_selectedSquare!.Piece, squareClicked.Piece);

            // A bit of Redundancy, But I can't fix it without refactoring the whole thing
            if (IsPawnPromotionMove(_playerMove.InitPiece, _playerMove.DestPiece))
            {
                ShowPawnPromotionDialog(IsWhiteTurn, async () => await ManageTurns());
                return;
            }
            else await ManageTurns();

        }

    }

    private bool RegisterMove(Move move)
    {

        if (!Board.MakeMove(move))
        {
            return false;
        }

        IsWhiteTurn = !IsWhiteTurn;

        UpdateGameState(move);
        
        return true;
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
            onPieceSelected!.Invoke();
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
            ((RelayCommand) piece.ClickCommand!).NotifyCanExecuteChanged();
        }
    }

    private bool CanClickSquare(SquareViewModel squareClicked)
    {
        if (!GameRunning)
        {
            return false;
        }

        if (_lockInput)
        {
            return false;
        }

        if (_selectedSquare is null && squareClicked.Piece != PieceType.Empty && squareClicked.Piece.IsWhite == IsWhiteTurn)
        {
            return true;
        }
        
        if (_selectedSquare is not null)
        {
            return true;
        }

        return false;
    }

    private void EndGame()
    {
        // TODO
    }

    private void LoadAiModule(AIDifficulty Difficulty) => _ai = new Ai(false, Difficulty);

    private void UpdateTurnText() => TurnText = IsWhiteTurn ? "White's turn!" : "Black's turn!";

    private void UpdateGameState(Move? move = null)
    {
        if (Board.IsCheckMate)
        {
            GameStateText = AppendMove(IsWhiteTurn ? "White is in Checkmate!" : "Black is in Checkmate!", move);
            GameRunning = false;
        }
        else if (Board.IsDraw)
        {
            GameStateText = AppendMove("It's a Draw!", move);
            GameRunning = false;
        }
        else if (Board.IsCheck)
        {
            GameStateText = AppendMove(IsWhiteTurn ? "White is in Check!" : "Black is in Check!", move);
        }
        else if (WhiteTimer <= TimeSpan.Zero)
        {
            GameStateText = "Time's up for White - Black wins!";
            GameRunning = false;
        }
        else if (BlackTimer <= TimeSpan.Zero)
        {
            GameStateText = "Time's up for Black - White wins!";
            GameRunning = false;
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
                NotifyCanClickSquares();
                UpdateGameState();
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