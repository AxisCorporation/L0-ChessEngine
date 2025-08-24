using System;
using System.Collections.Generic;
using Avalonia.Controls;
using L_0_Chess_Engine.Enums;

namespace L_0_Chess_Engine.Models;

/// <param name="initial">Starting coordinate</param>
/// <param name="destination">Destination Coordinate</param>
/// <param name="initPiece">Piece that belongs at the starting coordinate</param>
/// <param name="destPiece">Piece that belongs at the end coordinate</param>
public class Move(ChessPiece initPiece, ChessPiece destPiece, PieceType promotionPiece = PieceType.Empty)
{
    public bool IsValid { get => IsValidMove(); }

    // The message to be displayed when a move is invalidated
    public string ErrorMessage { get; set; } = "";

    public ChessPiece InitPiece { get; } = new ChessPiece(initPiece);
    public ChessPiece DestPiece { get; } = new ChessPiece(destPiece);

    public PieceType PromotionPiece { get; } = promotionPiece;
    
    public bool IsPromotion => PromotionPiece != PieceType.Empty;

    public bool TargetsKing
    {
        get
        {
            if (DestPiece.EqualsUncolored(PieceType.King))
            {
                ErrorMessage = "Cannot capture king!";
                return true;
            }

            return false;
        }
    }

    public bool IsEnPassant { get; set; }
    public bool IsCastling { get; set; }
    
    private static readonly Dictionary<PieceType, Func<Move, bool>> ValidationMap = [];
    
    private static readonly Dictionary<PieceType, Coordinate> AuraMap = [];

    static Move()
    {
        ValidationMap[PieceType.Pawn] = IsValidPawnMove;
        ValidationMap[PieceType.Knight] = IsValidKnightMove;
        ValidationMap[PieceType.Rook] = IsValidRookMove;
        ValidationMap[PieceType.Bishop] = IsValidBishopMove;
        ValidationMap[PieceType.Queen] = IsValidQueenMove;
        ValidationMap[PieceType.King] = IsValidKingMove;


        AuraMap[PieceType.Pawn] = new(1, 2);
        AuraMap[PieceType.Knight] = new(2, 2);
        AuraMap[PieceType.Rook] = new(8, 8);
        AuraMap[PieceType.Bishop] = new(8, 8);
        AuraMap[PieceType.Queen] = new(8, 8);
        AuraMap[PieceType.King] = new(1, 1);
    }

    

    public bool IsValidMove()
    {
        // We can take out the color to make the mapping simpler
        PieceType type = InitPiece.Type ^ InitPiece.Color;

        return ValidationMap[type](this);
    }
    
    private static bool IsValidGenericMove(Move move)
    {
        // False if the player just sets the piece back down in order to avoid turn skipping
        if (move.InitPiece.Coordinates == move.DestPiece.Coordinates)
        {
            return false;
        }

        // Cannot capture same team
        if (move.DestPiece != PieceType.Empty && (move.DestPiece.Color == move.InitPiece.Color))
        {
            move.ErrorMessage = "Path obstructed!";
            return false;
        }

        return true;
    }

    private static bool IsValidPawnMove(Move move)
    {
        if (!IsValidGenericMove(move))
        {
            return false;
        }

        (int initX, int initY) = move.InitPiece.Coordinates;
        (int destX, int destY) = move.DestPiece.Coordinates;

        bool IsValidDiagonal = Math.Abs(destX - initX) == 1;
        bool IsValidForward = destX == initX && move.DestPiece == PieceType.Empty;

        if (move.InitPiece.IsWhite)
        {
            if (!move.InitPiece.HasMoved)
            {
                IsValidForward &= destY == initY + 1 || (destY == initY + 2 && ChessBoard.Instance.Grid[destX, destY + 1] == PieceType.Empty);
            }
            else
            {
                IsValidForward &= destY == initY + 1;
            }

            IsValidDiagonal &= destY == initY + 1 && (move.DestPiece.IsValidPassantPlacement || move.DestPiece != PieceType.Empty);
        }
        else
        {
            if (!move.InitPiece.HasMoved)
            {
                IsValidForward &= destY == initY - 1 || (destY == initY - 2 && ChessBoard.Instance.Grid[destX, destY - 1] == PieceType.Empty);
            }
            else
            {
                IsValidForward &= destY == initY - 1;
            }

            IsValidDiagonal &= destY == initY - 1 && (move.DestPiece.IsValidPassantPlacement || move.DestPiece != PieceType.Empty);

        }

        move.IsEnPassant = IsValidDiagonal && move.DestPiece.IsValidPassantPlacement;

        return IsValidForward || IsValidDiagonal;
    }


    private static bool IsValidKnightMove(Move move)
    {
        if (!IsValidGenericMove(move))
        {
            return false;
        }

        (int initX, int initY) = move.InitPiece.Coordinates;
        (int destX, int destY) = move.DestPiece.Coordinates;

        return (Math.Abs(destX - initX) == 2 && Math.Abs(destY - initY) == 1) ||
               (Math.Abs(destX - initX) == 1 && Math.Abs(destY - initY) == 2);
    }


    private static bool IsValidRookMove(Move move)
    {
        if (!IsValidGenericMove(move))
        {
            return false;
        }


        (int InitX, int InitY) = move.InitPiece.Coordinates;
        (int DestX, int DestY) = move.DestPiece.Coordinates;

        bool IsValidHorizontal = InitY == DestY;
        bool IsValidVertical = InitX == DestX;


        if (IsValidHorizontal)
        {
            int start = Math.Min(InitX, DestX) + 1;
            int end = Math.Max(InitX, DestX);

            for (int x = start; x < end; x++)
            {
                if (ChessBoard.Instance.Grid[x, InitY].Type != PieceType.Empty)
                {
                    return false;
                }
            }
        }
        else if (IsValidVertical)
        {
            int start = Math.Min(InitY, DestY) + 1;
            int end = Math.Max(InitY, DestY);

            for (int y = start; y < end; y++)
            {
                if (ChessBoard.Instance.Grid[InitX, y].Type != PieceType.Empty)
                {
                    return false;
                }
            }
        }
        else
        {
            return false;
        }


        return true;
    }

    private static bool IsValidBishopMove(Move move)
    {
        if (!IsValidGenericMove(move))
        {
            return false;
        }

        (int InitX, int InitY) = move.InitPiece.Coordinates;
        (int DestX, int DestY) = move.DestPiece.Coordinates;

        bool IsValidDiagonal = Math.Abs(DestX - InitX) == Math.Abs(DestY - InitY);

        if (!IsValidDiagonal)
        {
            return false;
        }

        int xDirection = DestX > InitX ? 1 : -1;
        int yDirection = DestY > InitY ? 1 : -1;

        int x = InitX + xDirection;
        int y = InitY + yDirection;

        while (x != DestX && y != DestY)
        {
            if (ChessBoard.Instance.Grid[x, y].Type != PieceType.Empty)
            {
                return false;
            }

            x += xDirection;
            y += yDirection;
        }

        return true;
    }

    private static bool IsValidQueenMove(Move move)
    {
        return IsValidBishopMove(move) || IsValidRookMove(move);
    }

    private static bool IsValidKingMove(Move move)
    {
        if (!IsValidGenericMove(move))
        {
            return false;
        }

        (int InitX, int InitY) = move.InitPiece.Coordinates;
        (int DestX, int DestY) = move.DestPiece.Coordinates;

        int diffX = Math.Abs(DestX - InitX);
        int diffY = Math.Abs(DestY - InitY);

        //King moves 1 square in any direction
        if (diffX <= 1 && diffY <= 1)
        {
            return true;
        }

        // Castling conditions: king moves 2 squares horizontally and hasn't moved
        if (diffY == 0 && diffX == 2 && !move.InitPiece.HasMoved)
        {
            bool isKingSide = DestX > InitX;
            int rookX = isKingSide ? 7 : 0;
            ChessPiece rook = ChessBoard.Instance.Grid[rookX, InitY];

            if (rook.Type == PieceType.Empty || !rook.EqualsUncolored(PieceType.Rook) || rook.Color != move.InitPiece.Color || rook.HasMoved)
            {
                return false;
            }
            
            // Check if squares between king and rook are empty
            int min = Math.Min(InitX, rookX) + 1;
            int max = Math.Max(InitX, rookX);
            for (int x = min; x < max; x++)
            {
                if (ChessBoard.Instance.Grid[x, InitY].Type != PieceType.Empty)
                    return false;
            }

            // Check for check on current and intermediate squares
            if (ChessBoard.Instance.IsCheck)
                return false;

            int[] kingPath = isKingSide ? [InitX + 1, InitX + 2] : [InitX - 1, InitX - 2];

            foreach (int x in kingPath)
            {
                var simulatedMove = new Move(move.InitPiece, ChessBoard.Instance.Grid[x, InitY]);
                if (ChessBoard.Instance.WouldCauseCheck(simulatedMove))
                    return false;
            }

            move.IsCastling = true;
            return true;
        }

        return false;
    }

    public static List<Move> GeneratePieceMoves(ChessPiece piece)
    {
        PieceType type = piece.Type ^ piece.Color;

        (int auraX, int auraY) = AuraMap[type];

        List<Move> validMoves = [];

        for (int dX = -auraX; dX <= auraX; dX++)
        {
            for (int dY = -auraY; dY <= auraY; dY++)
            {
                int totalX = dX + piece.Coordinates.X;
                int totalY = dY + piece.Coordinates.Y;

                if (totalX < 0 || totalX >= 8 || totalY < 0 || totalY >= 8)
                {
                    continue;
                }

                if (type == PieceType.Rook && dX != 0 && dY != 0) // skip any diagonal moves for rook - automatically invalid
                {
                    continue;
                }
                
                if ((piece.Type == (PieceType.Pawn | PieceType.White) && totalY == 7 && dY == 1) || (piece.Type == (PieceType.Pawn | PieceType.Black) && totalY == 0 && dY == -1))
                {
                    
                    Move moveKnight = new(piece, ChessBoard.Instance.Grid[totalX, totalY], PieceType.Knight | (piece.IsWhite ? PieceType.White : PieceType.Black));
                    Move moveBishop = new(piece, ChessBoard.Instance.Grid[totalX, totalY], PieceType.Bishop | (piece.IsWhite ? PieceType.White : PieceType.Black));
                    Move moveRook = new(piece, ChessBoard.Instance.Grid[totalX, totalY], PieceType.Rook | (piece.IsWhite ? PieceType.White : PieceType.Black));
                    Move moveQueen = new(piece, ChessBoard.Instance.Grid[totalX, totalY], PieceType.Queen | (piece.IsWhite ? PieceType.White : PieceType.Black));
                    
                    if (moveKnight.IsValid)
                    {
                        validMoves.Add(moveKnight);
                        validMoves.Add(moveBishop);
                        validMoves.Add(moveRook);
                        validMoves.Add(moveQueen);
                    }
                    
                    continue;
                    
                }

                Move move = new(piece, ChessBoard.Instance.Grid[totalX, totalY]);

                if (move.IsValid)
                {
                    validMoves.Add(move);
                }
            }
        }

        return validMoves;
    }

    public static string CoordinateToString(Coordinate coordinate)
    {
        char column = ' ';
        int row = coordinate.Y + 1;

        for (int i = 0; i <= coordinate.X; i++)
        {
            column = (char)(65 + i);
        }

        return column + row.ToString();
    }

    public string GeneratePieceMovedMessage()
    {
        string piece = (InitPiece.Type ^ InitPiece.Color).ToString();
        string destination = CoordinateToString(DestPiece.Coordinates);

        return $"({InitPiece.Color}) {piece} to {destination}";
    }
    
}