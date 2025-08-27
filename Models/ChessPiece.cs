using System;
using L_0_Chess_Engine.Enums;

namespace L_0_Chess_Engine.Models;


public record struct Coordinate(int X, int Y);

public class ChessPiece 
{
    public PieceType Type { get; set; }
    public Coordinate Coordinates { get; set; }
    public bool HasMoved { get; set; }
    public bool IsEnPassantSquare { get; set; }
    public bool IsWhite { get => Color == PieceType.White; } 
    public PieceType Color { get; private set; }

    // Constructors

    public ChessPiece()
    {
        Color = PieceType.White;
        Type = PieceType.Pawn | Color;

        Coordinates = new(0, 0);

        HasMoved = false;
        IsEnPassantSquare = false;
    }

    public ChessPiece(PieceType type, Coordinate coordinates)
    {
        Type = type;
        Coordinates = coordinates;

        HasMoved = false;
        IsEnPassantSquare = false;

        Color = GetColor();
    }

    public ChessPiece(ChessPiece other)
    {
        Type = other.Type;
        Coordinates = other.Coordinates;
        HasMoved = other.HasMoved;
        IsEnPassantSquare = other.IsEnPassantSquare;
        Color = other.Color;
    }

    private PieceType GetColor()
    {
        PieceType mask = PieceType.White | PieceType.Black;
        if ((Type & mask) == PieceType.White)
        {
            return PieceType.White;
        }
        else
        {
            return PieceType.Black;
        }
    }
    
    public bool EqualsUncolored(PieceType type) => (Type ^ Color) == type; 
}