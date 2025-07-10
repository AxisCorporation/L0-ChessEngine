using System;

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
public record Coordinate(int X, int Y);

public class ChessPiece 
{
    public PieceType Type { get; set; }
    public Coordinate Coordinates { get; set; }
    public bool HasMoved { get; set; }
    public bool IsValidPassantPlacement { get; set; }
    public bool IsWhite { get => (Type & PieceType.White) == PieceType.White; }

    // Constructors

    public ChessPiece()
    {
        Type = PieceType.White | PieceType.Pawn;
        Coordinates = new(0, 0);
        
        HasMoved = false;
        IsValidPassantPlacement = false;
    }
    
    public ChessPiece(PieceType type, Coordinate coordinates)
    {
        Type = type;
        Coordinates = coordinates;

        HasMoved = false;
        IsValidPassantPlacement = false;
    }

    public bool EqualsUncolored(PieceType type) => IsWhite ? (Type ^ PieceType.White) == type : (Type ^ PieceType.Black) == type; 

    public static bool operator ==(ChessPiece A, PieceType B) => A.Type == B;
    
    public static bool operator !=(ChessPiece A, PieceType B) => A.Type != B;
    

    public static bool operator ==(PieceType A, ChessPiece B) => B == A;
    
    public static bool operator !=(PieceType A, ChessPiece B) => B != A;
    
}