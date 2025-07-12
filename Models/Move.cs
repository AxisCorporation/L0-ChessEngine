using System;
using System.Collections.Generic;

namespace L_0_Chess_Engine.Models;

public class Move
{
    public bool IsValid { get => IsValidMove(); }

    public ChessPiece InitPiece { get; set; }
    public ChessPiece DestPiece { get; set; }

    public bool TargetsKing { get => DestPiece.EqualsUncolored(PieceType.King); }

    public bool IsEnPassant { get; set; }
    public bool IsCastling { get; set; }

    /// <summary>
    /// Takes in an uncolored piecetype, and returns a function that takes in a Move object,
    /// and returns true if the move is valid
    /// </summary>
    private static Dictionary<PieceType, Func<Move, bool>> ValidationMap = [];

    /// <summary>
    /// Takes in an uncolored piecetype, and returns a coordinate object that 
    /// represents the absolute units that the piece is allowed to move
    /// </summary>
    private static Dictionary<PieceType, Coordinate> AuraMap = [];

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

    /// <param name="initial">Starting coordinate</param>
    /// <param name="destination">Destination Coordinate</param>
    /// <param name="initPiece">Piece that belongs at the starting coordinate</param>
    /// <param name="destPiece">Piece that belongs at the end coordinate</param>
    public Move(ChessPiece initPiece, ChessPiece destPiece)
    {
        InitPiece = initPiece;
        DestPiece = destPiece;
    }

    public bool IsValidMove()
    {
        // We can take out the color to make the mapping simpler
        PieceType type = InitPiece.IsWhite ? (InitPiece.Type ^ PieceType.White) : (InitPiece.Type ^ PieceType.Black);

        return ValidationMap[type](this);
    }

    /// <summary> Helper function for checking the bare minimum requirements of a valid move. </summary>
    /// <returns>False if
    /// <list type="bullet">
    /// <item> 
    /// <description> Initial coordinates are equal to destination </description> 
    /// </item>
    /// <item>
    /// <description> Destination grid piece is not empty, and is the same color as the initial piece </description>
    /// </item>
    /// </list>
    /// </returns>
    private static bool IsValidGenericMove(Move move)
    {
        // False if the player just sets the piece back down in order to avoid turn skipping
        if (move.InitPiece.Coordinates == move.DestPiece.Coordinates)
        {
            return false;
        }

        // Cannot capture same team
        if (move.DestPiece != PieceType.Empty && (move.DestPiece.IsWhite == move.InitPiece.IsWhite))
        {
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
                IsValidForward &= destY == initY + 1 || destY == initY + 2;
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
                IsValidForward &= destY == initY - 1 || destY == initY - 2;
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

        // Not necessary but it makes writing code easier and more readable
        (int initX, int initY) = move.InitPiece.Coordinates;
        (int destX, int destY) = move.DestPiece.Coordinates;

        if (Math.Abs(destX - initX) == 2 && Math.Abs(destY - initY) == 1)
        {
            return true;
        }
        if (Math.Abs(destX - initX) == 1 && Math.Abs(destY - initY) == 2)
        {
            return true;
        }

        return false;
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
        else if (!IsValidHorizontal && !IsValidVertical)
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
            move.IsCastling = true;
            return true;
        }

        //Not a valid king move
        return false;
    }

    public static List<Move> GetPossibleMoves(ChessPiece piece)
    {
        PieceType color = piece.Color;
        PieceType oppositeColor = piece.IsWhite ? PieceType.Black : PieceType.White;

        PieceType type = piece.Type ^ color;

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

                Move move = new(piece, ChessBoard.Instance.Grid[totalX, totalY]);

                Console.WriteLine($"MOVE CREATED: {move.InitPiece.Type}: {move.InitPiece.Coordinates} | {move.DestPiece.Coordinates}");

                if (move.IsValid)
                {
                    validMoves.Add(move);
                }
            }
        }

        return validMoves;
    }

}