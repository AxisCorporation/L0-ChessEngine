using System;

namespace L_0_Chess_Engine.Enums;
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