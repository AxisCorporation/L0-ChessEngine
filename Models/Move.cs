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

    /// <summary>
    /// Takes in an uncolored piecetype, and returns a function that takes in a Move object and a boolean parameter "IsWhite" as its parameters,
    /// and returns true if the move is valid
    /// </summary>
    private static Dictionary<PieceType, Func<Move, bool, bool>> ValidationMap = [];
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
        // We can take out the color to make the mapping simpler, and pass in the color as a bool if needed
        PieceType Type = InitPiece.IsWhite ? (InitPiece.Type | PieceType.White) : (InitPiece.Type | PieceType.Black);
        
        return ValidationMap[Type](this, InitPiece.IsWhite);
    }

    private static bool IsValidPawnMove(Move move, bool IsWhite)
    {
        return false;
    }
}
