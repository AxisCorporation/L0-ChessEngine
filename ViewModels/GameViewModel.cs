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
using System.Collections.Generic;

namespace L_0_Chess_Engine.ViewModels;

public partial class GameViewModel : ObservableObject
{
    private MainWindow _mainWindow;
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

    private ChessBoard Board { get; } = ChessBoard.Instance;

    private Move? _playerMove = null;
    PieceType Winner; // .White or .Black (not my favorite implementation)

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

    public ObservableCollection<SquareViewModel> GridPieces { get; set; } = [];
    public ObservableCollection<string> MovesCN { get; set; } = []; // A collection of all moves played in proper chess notation


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

    public GameViewModel(int timeLimit, bool LoadAi, AIDifficulty Difficulty, MainWindow MainWindow)
    {
        _mainWindow = MainWindow;

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
        // Player Move
        if (_playerMove is not null)
        {
            // Resetting Selection
            ResetHighlights();

            _selectedSquare = null;

            // Check if it's a Pawn Promotion Move

            if (_playerMove.InitPiece.EqualsUncolored(PieceType.Pawn) && (_playerMove.DestPiece.Coordinates.Y == 7 || _playerMove.DestPiece.Coordinates.Y == 0) && _playerMove.IsValid)
            {
                LockInput = true;
                // Ask player which piece they want
                var promotionResult = await PawnPromotion(IsWhiteTurn);

                // Replace old move with new move that includes promotion info
                _playerMove = new Move(_playerMove.InitPiece, _playerMove.DestPiece, promotionResult);
                LockInput = false;
            }

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
                Console.WriteLine($"Type: {aiMove.InitPiece.Type}");
                GameStateText = "AI Made Invalid Move: " + AppendMove("", aiMove);
                EndGame();
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
            HighlightSquare(squareClicked);
            NotifyCanClickSquares();
        }
        else
        {
            _playerMove = new(_selectedSquare!.Piece, squareClicked.Piece);

            // A bit of Redundancy, But I can't fix it without refactoring the whole thing
            await ManageTurns();

        }

    }

    private void HighlightSquare(SquareViewModel clickedSquare)
    {
        clickedSquare.IsSelected = true;

        List<Move> moves = Move.GeneratePieceMoves(clickedSquare.Piece);

        foreach (Move move in moves)
        {
            (int col, int row) = move.DestPiece.Coordinates;

            GridPieces[((7 - row) * 8) + col].IsSelected = true;
        }
    }

    private void ResetHighlights()
    {
        for (int i = 0; i < 64; i++)
        {
            GridPieces[i].IsSelected = false;
        }
    }

    private bool RegisterMove(Move move)
    {
        if (!Board.MakeMove(move))
        {
            return false;
        }

        IsWhiteTurn = !IsWhiteTurn;

        UpdateGameState();

        MovesCN.Add(MoveToCN(move));
        return true;
    }


    private async Task<PieceType> PawnPromotion(bool isWhite)
    {
        Window promotionView = new PawnPromotionView(isWhite);
        PawnPromotionViewModel viewModel = (PawnPromotionViewModel)promotionView.DataContext!;

        // If user closes the window via “X”, pick a default (Queen)
        promotionView.Closed += (_, __) => viewModel.EnsureDefaultIfNotChosen();

        promotionView.Show(); // modeless (UI still responsive)
        var piece = await viewModel.Completion; // wait for SelectPiece(...)
        promotionView.Close(); // will no-op if already closed

        return piece;
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
        string gameOverText = IsWhiteTurn ? "Black Won!" : "White Won!";
        var gameOverView = new GameOverView(gameOverText, _mainWindow);

        gameOverView.Show();
    }

    private void LoadAiModule(AIDifficulty Difficulty) => _ai = new Ai(false, Difficulty);

    private void UpdateTurnText() => TurnText = IsWhiteTurn ? "White's turn!" : "Black's turn!";

    private void UpdateGameState()
    {
        if (Board.IsCheckMate || Board.IsDraw)
        {
            GameRunning = false;
        }

        else if (WhiteTimer <= TimeSpan.Zero)
        {
            Winner = PieceType.White;
            GameRunning = false;
        }
        else if (BlackTimer <= TimeSpan.Zero)
        {
            Winner = PieceType.Black;
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

    private static string AppendMove(string Message, Move? move) // debug
    {
        if (!string.IsNullOrWhiteSpace(Message))
        {
            Message = Message.Insert(0, "- ");
        }

        return move is not null ? $"{move.GeneratePieceMovedMessage()} {Message}" : Message;
    }

    private string MoveToCN(Move move)
    {
        string ChessNotation = $"{MovesCN.Count}. ";
        char? pieceChar = (move.InitPiece.Type ^ move.InitPiece.Color) switch
        {
            PieceType.Knight => 'N',
            PieceType.Bishop => 'B',
            PieceType.Rook => 'R',
            PieceType.Queen => 'Q',
            PieceType.King => 'K',

            _ => null // Pawn has no abbreviation
        };

        ChessNotation += $"{pieceChar}";

        if (move.DestPiece != PieceType.Empty)
        {
            ChessNotation += 'x';
        }

        ChessNotation += $"{Move.CoordinateToString(move.DestPiece.Coordinates).ToLower()}";

        if (Board.IsCheckMate)
        {
            ChessNotation += '#';
        }
        else if (Board.IsCheck)
        {
            ChessNotation += '+';
        }

        return ChessNotation;
    }
}