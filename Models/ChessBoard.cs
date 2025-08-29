using System;
using System.Linq;
using System.Collections.Generic;
using L_0_Chess_Engine.Enums;
using Avalonia.Controls;
using System.Threading.Tasks;
using L_0_Chess_Engine.Common;
using System.Diagnostics;

namespace L_0_Chess_Engine.Models;

public class ChessBoard
{
    //Constant FEN for the starting position
    private const string DefaultFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
    private PieceType _promotedPieceType = PieceType.Empty;
    
    public ChessPiece[,] Grid { get; set; }
    public bool IsDraw { get; private set; }

    public bool IsCheck { get; private set; }

    public bool IsCheckMate { get; private set; }

    public bool IsWhiteTurn { get; set; }

    // Invoked every time grid updates
    public event Action? GridUpdated;

    private static ChessBoard? _instance;
    public static ChessBoard Instance
    {
        get => _instance ??= new ChessBoard();
    }


    private ChessBoard() // Constructor to initialize
    {
        IsWhiteTurn = true;

        Grid = new ChessPiece[8, 8];

        ResetBoard(); // Initialize the board
    }

    public void MakeMove(Move move)
    {
        // Reset all valid En Passant moves
        foreach (var piece in Grid)
        {
            piece.IsEnPassantSquare = false;
        }

        string SFXPath = GetSFX(move);
        (int initX, int initY) = move.InitPiece.Coordinates;
        (int destX, int destY) = move.DestPiece.Coordinates;

        ChessPiece pieceToMove = Grid[initX, initY];

        pieceToMove.HasMoved = true;

        if (move.Type == MoveType.Castling)
        {
            HandleCastling(move);
        }
        else if (move.InitPiece.EqualsUncolored(PieceType.Pawn))
        {
            CheckEnPassant(move, pieceToMove);
        }

        Grid[initX, initY] = new ChessPiece(PieceType.Empty, new(initX, initY));
        Grid[destX, destY] = pieceToMove;
        pieceToMove.Coordinates = new(destX, destY);

        if (move.Type == MoveType.Promotion)
        {
            Grid[destX, destY].Type = move.PromotionPiece;
        }

        IsWhiteTurn = !IsWhiteTurn;

        IsDraw = DrawScan();

        if (!IsDraw)
        {
            IsCheck = CheckScan();
            IsCheckMate = IsCheck && CheckMateScan(); // CheckmateScan is only called if game is in check
        }

        if (IsCheck)
        {
            SFXPath = SoundPlayer.CheckSFXPath;
        }
        else if (IsCheckMate)
        {
            SFXPath = SoundPlayer.CheckmateSFXPath;
        }

        _ = SoundPlayer.Play(SFXPath);
        GridUpdated?.Invoke();
    }

    private static string GetSFX(Move move)
    {
        if (move.Type == MoveType.Promotion)
        {
            return SoundPlayer.PromotionSFXPath;
        }

        if (move.Type == MoveType.EnPassant || move.Type == MoveType.Capture)
        {
            return SoundPlayer.CaptureSFXPath;
        }

        if (move.Type == MoveType.Castling)
        {
            return SoundPlayer.CastleSFXPath;
        }

        return SoundPlayer.MoveSFXPath;
    }

    public void ResetBoard()
    {
        ReadFEN(DefaultFEN);
        IsWhiteTurn = true;
    }

    private bool DrawScan()
    {
        // First check: Remaining material
        // 1a. King vs King
        // 1b. King and bishop vs king
        // 1c. King and bishop vs king and bishop
        int Combination = CountPiecesOfType(ConsiderColor: false, PieceType.King, PieceType.Bishop);
        if (Combination <= 4 && Grid.Length == Combination)
        {
            return true;
        }

        // 1d. King and knight vs king
        Combination = CountPiecesOfType(ConsiderColor: false, PieceType.King, PieceType.Knight);
        if (Combination == 3 && Grid.Length == Combination)
        {
            return true;
        }

        // 2. Stalemate
        List<Move> moves = [];

        foreach (var square in Grid)
        {
            if (square.Type == PieceType.Empty)
            {
                continue;
            }

            moves.AddRange(from move in Move.GeneratePieceMoves(square)
                           where !WouldCauseCheck(move)
                           select move);
        }

        return moves.Count == 0;
    }

    private int CountPiecesOfType(bool ConsiderColor, params PieceType[] Types)
    {
        int count = 0;
        foreach (var piece in Grid)
        {
            PieceType Type = ConsiderColor ? piece.Type : piece.Type ^ piece.Color;
            if (Types.Contains(Type))
            {
                count++;
            }
        }

        return count;
    }

    // After making a move, it checks the king of the opposing team to see if its in check
    private bool CheckScan()
    {
        PieceType Color = IsWhiteTurn ? PieceType.White : PieceType.Black;

        foreach (var piece in Grid)
        {

            if (piece.Type == PieceType.Empty || piece.Color == Color)
            {
                continue;
            }

            List<Move> moves = [..from m in Move.GeneratePieceMoves(piece)
                            where m.TargetsKing
                            select m]; // Gets all moves for piece where it can threaten the king

            if (moves.Count != 0)
            {
                return true;
            }
            
        }

        return false;
    }

    private bool CheckMateScan()
    {
        PieceType colorInCheck = IsWhiteTurn ? PieceType.White : PieceType.Black;
        List<Move> validMoves = [];

        foreach (var piece in Grid)
        {
            if (piece.Color != colorInCheck || piece.Type == PieceType.Empty)
            {
                continue;
            }

            validMoves.AddRange(from move in Move.GeneratePieceMoves(piece)
                                where
                                !WouldCauseCheck(move)
                                select move);
        }

        return validMoves.Count == 0;
    }

    public void HypotheticalMove(Move move)
    {
        (int initX, int initY) = move.InitPiece.Coordinates;
        (int destX, int destY) = move.DestPiece.Coordinates;

        ChessPiece originalInit = Grid[initX, initY];

        // Applying move
        Grid[destX, destY] = originalInit;
        Grid[initX, initY] = new ChessPiece(PieceType.Empty, new(initX, initY));
        originalInit.Coordinates = new(destX, destY);

        if (move.Type == MoveType.Promotion)
        {
            originalInit.Type = move.PromotionPiece;
        }
        
        IsDraw = DrawScan();

        if (!IsDraw)
        {
            IsCheck = CheckScan();
            IsCheckMate = IsCheck && CheckMateScan(); // CheckmateScan is only called if game is in check
        }
    }

    public void UndoHypotheticalMove(Move move)
    {
        (int initX, int initY) = move.InitPiece.Coordinates;
        (int destX, int destY) = move.DestPiece.Coordinates;

        ChessPiece originalInit = Grid[destX, destY];
        ChessPiece originalDest = move.DestPiece;

        // Undoing move
        Grid[initX, initY] = originalInit;
        Grid[destX, destY] = originalDest;

        originalDest.Coordinates = new(destX, destY);
        originalInit.Coordinates = new(initX, initY);
        
        if (move.Type == MoveType.Promotion)
        {
            originalInit.Type = PieceType.Pawn | (originalInit.IsWhite ? PieceType.White : PieceType.Black);
        }
    }

    private void CheckEnPassant(Move move, ChessPiece pieceToMove)
    {
        (int initX, int initY) = move.InitPiece.Coordinates;
        (int destX, int destY) = move.DestPiece.Coordinates;

        if (Math.Abs(destY - initY) == 2)
        {
            int PassantY = move.InitPiece.IsWhite ? destY - 1 : destY + 1;
            Grid[destX, PassantY].IsEnPassantSquare = true;
        }
        else if (move.Type == MoveType.EnPassant)
        {
            int CapturedPawnY = pieceToMove.IsWhite ? destY - 1 : destY + 1;
            Grid[destX, CapturedPawnY] = new ChessPiece(PieceType.Empty, new(destX, CapturedPawnY));
        }
    }
    
    public void HandleCastling(Move move)
    {
        (int initX, int initY) = move.InitPiece.Coordinates;
        (int destX, int destY) = move.DestPiece.Coordinates;

        ChessPiece king = move.InitPiece;

        // Determine if kingside or not
        bool isKingSide = destX > initX;

        // Rook's start and new positions
        int rookStartX = isKingSide ? 7 : 0;
        int rookNewX = isKingSide ? destX - 1 : destX + 1;

        ChessPiece rook = Grid[rookStartX, initY];

        // Move rook to new spot
        rook.Coordinates = new Coordinate(rookNewX, initY);
        Grid[rookNewX, initY] = rook;
        Grid[rookStartX, initY] = new ChessPiece(PieceType.Empty, new(rookStartX, initY));

        king.HasMoved = true;
        rook.HasMoved = true;
    }

    public bool WouldCauseCheck(Move move)
    {
        (int initX, int initY) = move.InitPiece.Coordinates;
        (int destX, int destY) = move.DestPiece.Coordinates;

        ChessPiece originalInit = Grid[initX, initY];
        ChessPiece originalDest = Grid[destX, destY];

        // Apply simulated move
        Grid[destX, destY] = originalInit;
        Grid[initX, initY] = new ChessPiece(PieceType.Empty, new(initX, initY));
        originalInit.Coordinates = new(destX, destY);

        bool causesCheck = CheckScan(); // Will return true if king is in check

        // Undo move
        Grid[initX, initY] = originalInit;
        Grid[destX, destY] = originalDest;
        originalInit.Coordinates = new(initX, initY);
        originalDest.Coordinates = new(destX, destY); // if needed
        
        return causesCheck;
    }

    public bool ReadFEN(string fen)
    {
        // Split the layout into rows (8 rows for an 8x8 board)
        var rows = fen.Split('/');
        if (rows.Length != 8)
            return false; // 8 rows must or invalid

        // Iterate through each row and parse it
        for (int y = 7; y >= 0; y--)
        {

            var row = rows[7 - y];
            int currentCol = 0;

            foreach (var c in row)
            {
                if (char.IsDigit(c))
                {
                    // Character that are numbers show empty squares
                    int emptySquares = int.Parse(c.ToString());

                    // Add empty squares to the current row
                    for (int j = 0; j < emptySquares; j++)
                    {
                        Grid[currentCol, y] = new ChessPiece(PieceType.Empty, new(currentCol, y));
                        currentCol++;
                    }
                }
                else
                {
                    // Switch statement to handle piece types
                    PieceType? pieceType = c switch
                    {
                        'p' => PieceType.Pawn | PieceType.Black,
                        'P' => PieceType.Pawn | PieceType.White,

                        'r' => PieceType.Rook | PieceType.Black,
                        'R' => PieceType.Rook | PieceType.White,

                        'n' => PieceType.Knight | PieceType.Black,
                        'N' => PieceType.Knight | PieceType.White,

                        'b' => PieceType.Bishop | PieceType.Black,
                        'B' => PieceType.Bishop | PieceType.White,

                        'q' => PieceType.Queen | PieceType.Black,
                        'Q' => PieceType.Queen | PieceType.White,

                        'k' => PieceType.King | PieceType.Black,
                        'K' => PieceType.King | PieceType.White,

                        // Invalid piece character
                        _ => null
                    };

                    if (pieceType is null)
                    {
                        return false;
                    }

                    // Create the chess piece with the correct color and type
                    Grid[currentCol, y] = new ChessPiece(pieceType.Value, new(currentCol, y));
                    currentCol++;
                }
            }
        }

        // Return true when the board is successfully set up
        return true;
    }

    public void SetPromotedPieceType(PieceType pieceType)
    {
        _promotedPieceType = pieceType;
    }

    public ChessPiece GetPieceFromPromotion()
    {
        if (_promotedPieceType == PieceType.Empty)
        {
            throw new InvalidOperationException("Promoted piece type is not set.");
        }
        
        // Coordinates are set later
        return new ChessPiece(_promotedPieceType, new(0, 0));
    }
}
