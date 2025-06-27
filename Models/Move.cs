using System;
using System.Collections.Generic;
using L_0_Chess_Engine.Contracts;
namespace L_0_Chess_Engine.Models;

public class Move : IMove
{
    public bool IsValid { get => IsValidMove(); }
    public Coordinate Initial { get; set; }
    public Coordinate Destination { get; set; }

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

        // Add rest of the validation checks here
        // Ex: 
        // ValidationMap[PieceType.Rook] = IsValidRookMove;
    }

    /// <param name="initial">Starting coordinate</param>
    /// <param name="destination">Destination Coordinate</param>
    /// <param name="initPiece">Piece that belongs at the starting coordinate</param>
    /// <param name="destPiece">Piece that belongs at the end coordinate</param>
    public Move(Coordinate initial, Coordinate destination, ChessPiece initPiece, ChessPiece destPiece)
    {
        Initial = initial;
        Destination = destination;
        InitPiece = initPiece;
        DestPiece = destPiece;
    }

    public bool IsValidMove()
    {
        // We can take out the color to make the mapping simpler
        PieceType Type = InitPiece.IsWhite ? (InitPiece.Type ^ PieceType.White) : (InitPiece.Type ^ PieceType.Black);
        
        return ValidationMap[Type](this);
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
        if (move.Initial == move.Destination)
        {
            return false;
        }

        if (move.DestPiece.IsWhite && move.InitPiece.IsWhite)
        {
            return false;
        }

        if (!move.DestPiece.IsWhite && !move.InitPiece.IsWhite)
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

        (int InitX, int InitY) = move.Initial;
        (int DestX, int DestY) = move.Destination;

        bool IsValidDiagonal = Math.Abs(DestX - InitX) == 1;
        bool IsValidForward = DestX == InitX;

        if (move.InitPiece.IsWhite)
        {
            if (move.InitPiece.HasMoved)
            {
                IsValidForward &= DestY == InitY + 1 || DestY == InitY + 2;
            }
            else
            {
                IsValidForward &= DestY == InitY + 1;
            }

            IsValidDiagonal &= DestY == InitY + 1 && (move.DestPiece.IsValidPassantPlacement || move.DestPiece != PieceType.Empty);

        }
        else
        {
            if (move.InitPiece.HasMoved)
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
}
