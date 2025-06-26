using System;
using L_0_Chess_Engine.Contracts;

namespace L_0_Chess_Engine.Models;

[Flags]
public enum PieceType
{
    Empty,
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King,

    // Colour
    White = 8,
    Black = 16
}


public class ChessPiece : IChessPiece
{
    public PieceType Type { get; set; }
    public bool AtStart { get; set; }
    public bool IsWhite
    {
        get => (Type & PieceType.White) == PieceType.White;
    }

    // Constructors

    public ChessPiece()
    {
        Type = PieceType.White | PieceType.Pawn;
        AtStart = true;
    }

    public ChessPiece(PieceType type)
    {
        Type = type;
    }

    public static bool operator ==(ChessPiece A, PieceType B) => A.Type == B;
    

    public static bool operator !=(ChessPiece A, PieceType B) => A.Type != B;
    

    public static bool operator ==(PieceType A, ChessPiece B) => B == A;
    
    public static bool operator !=(PieceType A, ChessPiece B) => B != A;
    
}