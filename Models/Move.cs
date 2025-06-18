using L_0_Chess_Engine.Contracts;
namespace L_0_Chess_Engine.Models;

public class Move : IMove
{
    public IChessPiece Piece { get; set; }
    public Coordinate Initial { get; set; }
    public Coordinate Destination { get; set; }

    public Move(IChessPiece piece, Coordinate initial, Coordinate destination)
    {
        Piece = piece;
        Initial = initial;
        Destination = destination;
    }

    public bool IsValidMove()
    {
        return true;
    }
}
