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
using System.Text;
using System.Linq;

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
    PieceType Winner; // PieceType.White or .Black (not my favorite implementation)

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



        if (LoadAi)
        {
            LoadAiModule(Difficulty);
            _aiGame = true;
        }

        Board.GridUpdated += UpdateGrid;

        IsWhiteTurn = true;
        GameRunning = true;

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
        clickedSquare.IsHighlighted = true;

        List<Move> moves = Move.GeneratePieceMoves(clickedSquare.Piece);

        foreach (Move move in moves)
        {
            if (Board.WouldCauseCheck(move))
            {
                continue;
            }

            (int col, int row) = move.DestPiece.Coordinates;

            GridPieces[((7 - row) * 8) + col].IsHighlighted = true;
        }
    }

    private void ResetHighlights()
    {
        for (int i = 0; i < 64; i++)
        {
            GridPieces[i].IsHighlighted = false;
        }
    }

    private bool RegisterMove(Move move)
    {
        if (!move.IsValid)
        {
            return false;
        }

        UpdateMoveList(move);

        if (!Board.MakeMove(move))
        {
            return false;
        }

        UpdateGameState();

        IsWhiteTurn = !IsWhiteTurn;
        return true;
    }


    private static async Task<PieceType> PawnPromotion(bool isWhite)
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
        if (Board.IsDraw)
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

        // I hate this implementation but its ok
        string LastMove = MovesCN[^1][^1] == ' ' ? MovesCN[^1][..(MovesCN[^1].Length - 3)] : MovesCN[^1];

        if (Board.IsCheck)
        {
            LastMove += '+';
        }
        else if (Board.IsCheckMate)
        {
            LastMove += '#';
            GameRunning = false;
        }

        MovesCN[^1] = MovesCN[^1][^1] == ' ' ? LastMove + " | " : LastMove; 
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

    private void UpdateMoveList(Move move)
    {
        if (!IsWhiteTurn)
        {
            MovesCN[^1] += $"{MoveToCN(move)}";
        }
        else
        {
            MovesCN.Add($"{MovesCN.Count + 1}. {MoveToCN(move)} | ");
        }
    }

    private string MoveToCN(Move move)
    {
        StringBuilder moveCN = new();

        (int initX, int initY) = move.InitPiece.Coordinates;
        (int destX, int destY) = move.DestPiece.Coordinates;

        if (move.IsCastling)
        {
            if (destX > initX) // Kingside
            {
                moveCN.Append("O-O");
            }
            else if (destX < destY) // Queenside
            {
                moveCN.Append("O-O-O");
            }

            return moveCN.ToString();
        }

        char? pieceChar = (move.InitPiece.Type ^ move.InitPiece.Color) switch
        {
            PieceType.Knight => 'N',
            PieceType.Bishop => 'B',
            PieceType.Rook => 'R',
            PieceType.Queen => 'Q',
            PieceType.King => 'K',

            _ => null // Pawn has no abbreviation
        };

        moveCN.Append($"{pieceChar}");

        // If a piece of the same type can also move onto the same square, we must additionally denote its file, or its rank if the file is the same
        // This block of code is extremely intensive just to calculate an extra character, we can remove it if needed
        foreach (var piece in Board.Grid)
        {
            if (piece.Color != move.InitPiece.Color || piece.Coordinates == move.InitPiece.Coordinates || !piece.EqualsUncolored(move.InitPiece.Type ^ move.InitPiece.Color))
            {
                Debug.WriteLine("Continuing");
                continue;
            }

            bool isAmbiguous = (from moves in Move.GeneratePieceMoves(piece)
                                where moves.DestPiece.Coordinates == move.DestPiece.Coordinates
                                select moves).Any();

            if (!isAmbiguous)
            {
                continue;
            }

            if (piece.Coordinates.X != initX)
            {
                moveCN.Append(char.ToLower(Move.CoordinateToString(move.InitPiece.Coordinates)[0])); // This gets the file/column
            }
            else if (piece.Coordinates.X == initX)
            {
                moveCN.Append(Move.CoordinateToString(move.InitPiece.Coordinates)[1]); // This gets the row/rank
            }
            else
            {
                Debug.WriteLine($"Piece coordinates: {piece.Coordinates} | Coordinates of moving object: {move.InitPiece.Coordinates}");
                moveCN.Append(Move.CoordinateToString(move.InitPiece.Coordinates));
            }

            break;
        }

        if (move.DestPiece != PieceType.Empty)
        {
            moveCN.Append('x');
        }

        moveCN.Append($"{Move.CoordinateToString(move.DestPiece.Coordinates).ToLower()}");

        return moveCN.ToString();
    }
}