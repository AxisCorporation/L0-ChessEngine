using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using L_0_Chess_Engine.Contracts;
namespace L_0_Chess_Engine.Models;

public class Move : IMove
{
    public bool IsValid { get => IsValidMove(); }


    // Rather than have two separate chesspieces AND coordinate objects here, it might also be better to 
    // just store the coordinates inside the chesspieces, but this would require refactoring
    public ChessPiece InitPiece { get; set; }
    public ChessPiece DestPiece { get; set; }

    public bool IsEnPassant { get; set; }

    /// <summary>
    /// Takes in an uncolored piecetype, and returns a function that takes in a Move object,
    /// and returns true if the move is valid
    /// </summary>
    private static Dictionary<PieceType, Func<Move, bool>> ValidationMap = [];
    static Move()
    {
        ValidationMap[PieceType.Pawn] = IsValidPawnMove;
        ValidationMap[PieceType.Knight] = IsValidKnightMove;

        // Always false types
        ValidationMap[PieceType.Empty] = (m) => false;
        ValidationMap[PieceType.Black] = (m) => false;
        ValidationMap[PieceType.White] = (m) => false;
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
        if (move.InitPiece.Type == PieceType.Empty)
        {
            return false;
        }
        
        if (move.InitPiece.Coordinates == move.DestPiece.Coordinates)
        {
            return false;
        }

        if (move.DestPiece != PieceType.Empty && move.DestPiece.IsWhite == move.InitPiece.IsWhite)
        {
            Console.WriteLine("Issue here");
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

        (int InitX, int InitY) = move.InitPiece.Coordinates;
        (int DestX, int DestY) = move.DestPiece.Coordinates;

        bool IsValidDiagonal = Math.Abs(DestX - InitX) == 1;
        bool IsValidForward = DestX == InitX;

        if (move.InitPiece.IsWhite)
        {
            if (!move.InitPiece.HasMoved)
            {
                IsValidForward &= DestY == InitY + 1 || DestY == InitY + 2;
            }
            else
            {
                IsValidForward &= DestY == InitY + 1;
            }
            Console.WriteLine($"Step 2: {IsValidForward} | Dest Y : {DestY} | InitY : {InitY}");

            IsValidDiagonal &= DestY == InitY + 1 && (move.DestPiece.IsValidPassantPlacement || move.DestPiece != PieceType.Empty);

        }
        else
        {
            if (!move.InitPiece.HasMoved)
            {
                IsValidForward &= DestY == InitY - 1 || DestY == InitY - 2;
            }
            else
            {
                IsValidForward &= DestY == InitY - 1;
            }

            IsValidDiagonal &= DestY == InitY - 1 && (move.DestPiece.IsValidPassantPlacement || move.DestPiece != PieceType.Empty);
        }

        if (IsValidDiagonal && move.DestPiece.IsValidPassantPlacement)
        {
            move.IsEnPassant = true;
        }
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
}
