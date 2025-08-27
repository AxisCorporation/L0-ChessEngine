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

using Avalonia.Media.Imaging;
using Avalonia.Platform;
using L_0_Chess_Engine.Common;

namespace L_0_Chess_Engine.ViewModels;

public partial class GameViewModel : ObservableObject
{
    [ObservableProperty]
    private Bitmap _pawnPromotionRook;
    [ObservableProperty]
    private Bitmap _pawnPromotionKnight;
    [ObservableProperty]
    private Bitmap _pawnPromotionBishop;
    [ObservableProperty]
    private Bitmap _pawnPromotionQueen;
    [ObservableProperty]
    private bool _pawnPromotionActive = false;
    [ObservableProperty]
    private string _blackTimerText;
    [ObservableProperty]
    private string? _turnText;
    [ObservableProperty]
    private string _whiteTimerText;
    [ObservableProperty]
    private string _gameOverText;

    private bool _gameOver;
    public bool GameOver
    {
        get => _gameOver;
        set
        {
            _gameOver = value;
            GenerateGameOverText();
            OnPropertyChanged(nameof(GameOver));
        }
    }

    private PieceType _promotionPiece;

    public TimeSpan WhiteTimer { get; set; }

    public bool IsWhiteWinner = true;

    private TimeSpan BlackTimer { get; set; }

    private readonly string _timeFormat = @"m\:ss";

    private ChessBoard Board { get; } = ChessBoard.Instance;


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

    public GameViewModel(int timeLimit, bool LoadAi, AIDifficulty Difficulty)
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

                square.ClickCommand = new RelayCommand(async () => await OnSquareClick(square), () => CanClickSquare(square));

                GridPieces.Add(square);
            }
        }


        if (LoadAi)
        {
            LoadAiModule(Difficulty);
        }

        Board.GridUpdated += UpdateGrid;

        IsWhiteTurn = true;
        GameOver = false;

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

    private async Task ManageTurns(Move move)
    {
        // Player Move
        if (move is not null)
        {
            // Resetting Selection
            ResetHighlights();

            _selectedSquare = null;

            // Check if it's a Pawn Promotion Move
            if (move.InitPiece.EqualsUncolored(PieceType.Pawn) && (move.DestPiece.Coordinates.Y == 7 || move.DestPiece.Coordinates.Y == 0) && move.IsValid)
            {
                PawnPromotion(); // sets _promotionPiece

                while (PawnPromotionActive)
                {
                    await Task.Delay(50);
                }

                // Replace old move with new move that includes promotion info
                move = new Move(move.InitPiece, move.DestPiece, _promotionPiece);
            }

            if (!RegisterMove(move))
            {
                Debug.WriteLine("DEBUG: Invalid player move, resetting selection.");
                return;
            }
        }

        if (_ai is not null && !IsWhiteTurn)
        {
            LockInput = true;

            Move aiMove = await Task.Run(_ai.GenerateMove);
            RegisterMove(aiMove);

            LockInput = false;
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
            Move move = new(_selectedSquare!.Piece, squareClicked.Piece);

            // A bit of Redundancy, But I can't fix it without refactoring the whole thing
            await ManageTurns(move);
        }
    }

    [RelayCommand]
    private void Promotion(string Option)
    {
        _promotionPiece = IsWhiteTurn ? PieceType.White : PieceType.Black;

        _promotionPiece |= Option switch
        {
            "Rook" => PieceType.Rook,
            "Bishop" => PieceType.Bishop,
            "Knight" => PieceType.Knight,

            _ => PieceType.Queen
        };

        PawnPromotionActive = false;
        LockInput = false;
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
        if (!move.IsValid || Board.WouldCauseCheck(move) || move.TargetsKing)
        {
            _ = SoundPlayer.Play(SoundPlayer.IllegalMoveSFXPath);
            return false;
        }
                
        UpdateMoveList(move);

        Board.MakeMove(move);

        UpdateGameState();

        IsWhiteTurn = !IsWhiteTurn;
        return true;
    }


    private void PawnPromotion()
    {
        char colorPrefix = IsWhiteTurn ? 'W' : 'B';
        PawnPromotionBishop = new(AssetLoader.Open(new Uri($"avares://L-0 Chess Engine/Assets/Images/{colorPrefix}_Bishop.png")));
        PawnPromotionKnight = new(AssetLoader.Open(new Uri($"avares://L-0 Chess Engine/Assets/Images/{colorPrefix}_Knight.png")));
        PawnPromotionRook = new(AssetLoader.Open(new Uri($"avares://L-0 Chess Engine/Assets/Images/{colorPrefix}_Rook.png")));
        PawnPromotionQueen = new(AssetLoader.Open(new Uri($"avares://L-0 Chess Engine/Assets/Images/{colorPrefix}_Queen.png")));

        PawnPromotionActive = true;
        LockInput = true;
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
        if (GameOver)
        {
            return false;
        }

        if (LockInput)
        {
            return false;
        }

        if (_selectedSquare is null && squareClicked.Piece.Type != PieceType.Empty && squareClicked.Piece.IsWhite == IsWhiteTurn)
        {
            return true;
        }

        if (_selectedSquare is not null)
        {
            return true;
        }

        return false;
    }

    private void LoadAiModule(AIDifficulty Difficulty) => _ai = new Ai(false, Difficulty);

    private void UpdateTurnText() => TurnText = IsWhiteTurn ? "White's turn!" : "Black's turn!";

    private void UpdateGameState()
    {
        if (Board.IsDraw)
        {
            GameOver = true;
        }
        else if (WhiteTimer <= TimeSpan.Zero)
        {
            IsWhiteWinner = false;
            GameOver = true;
        }
        else if (BlackTimer <= TimeSpan.Zero)
        {
            IsWhiteWinner = true;
            GameOver = true;
        }

        // I hate this implementation but its ok
        string LastMove = MovesCN[^1][^1] == ' ' ? MovesCN[^1][..(MovesCN[^1].Length - 1)] : MovesCN[^1];

        if (Board.IsCheckMate)
        {
            IsWhiteWinner = !IsWhiteTurn; // If game ends and its black's turn, white wins and vice versa

            LastMove += '#';
            GameOver = true;
        }
        else if (Board.IsCheck)
        {
            LastMove += '+';
        }

        MovesCN[^1] = MovesCN[^1][^1] == ' ' ? LastMove + ' ' : LastMove;
    }

    private async Task UpdateTurnTimersAsync()
    {
        while (!GameOver)
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

            if (WhiteTimer == TimeSpan.FromSeconds(10) || BlackTimer == TimeSpan.FromSeconds(10))
            {
                _ = SoundPlayer.Play(SoundPlayer.TenSecondsLeftSFXPath);
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
        if (IsWhiteTurn)
        {
            MovesCN.Add($"{MovesCN.Count + 1}. {MoveToCN(move)} ");
        }
        else
        {
            MovesCN[^1] += $"| {MoveToCN(move)}";
        }
    }

    private string MoveToCN(Move move)
    {
        StringBuilder moveCN = new();

        (int initX, int initY) = move.InitPiece.Coordinates;
        (int destX, int destY) = move.DestPiece.Coordinates;

        if (move.Type == MoveType.Castling)
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
                moveCN.Append(Move.CoordinateToString(move.InitPiece.Coordinates));
            }

            break;
        }

        if (move.DestPiece.Type != PieceType.Empty)
        {
            moveCN.Append('x');
        }

        moveCN.Append($"{Move.CoordinateToString(move.DestPiece.Coordinates).ToLower()}");

        return moveCN.ToString();
    }

    private void GenerateGameOverText()
    {
        StringBuilder gameOver = new("Game over!\n");

        if (Board.IsDraw)
        {
            gameOver.Append("1/2-1/2");
        }
        else
        {
            gameOver.Append(IsWhiteWinner ? "1-0" : "0-1"); 
        }

        GameOverText = gameOver.ToString();
    }
}