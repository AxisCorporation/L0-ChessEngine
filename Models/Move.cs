using L_0_Chess_Engine.Contracts;
namespace L_0_Chess_Engine.Models;

public class Move : IMove
{
    public bool IsValid { get => IsValidMove(); }
    public Coordinate Initial { get; set; }
    public Coordinate Destination { get; set; }

    public Move(Coordinate initial, Coordinate destination)
    {
        Initial = initial;
        Destination = destination;
    }

    public bool IsValidMove()
    {
        return true;
    }
}
