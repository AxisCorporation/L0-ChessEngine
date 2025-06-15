using L_0_Chess_Engine.Contracts;

namespace L_0_Chess_Engine.Models;

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

    public bool IsWhite
    {
        get => (Type & PieceType.White) == PieceType.White;
    }

}