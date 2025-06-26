using L_0_Chess_Engine.Models;

namespace L_0_Chess_Engine.Contracts;

public record Coordinate(int X, int Y);

public interface IChessPiece
{
    public PieceType Type { get; set; }
    public bool IsWhite { get; }
    public bool HasMoved { get; set; }
    public bool IsValidPassantPlacement { get; set; }
}

public interface IChessBoard
{
    public IChessPiece[,] Grid { get; set; }
    public bool IsCheck { get; set; }
    public bool IsCheckMate { get; set; }

    public void MakeMove(IMove move);
    public void ResetBoard();

    protected bool ReadFEN(string fen);
}

public interface IGameManager
{
    public IChessBoard Board { get; set; }

    public bool StartGame();
    public bool EndGame();
}

public interface IMove
{
    public bool IsValid { get => IsValidMove(); }

    public Coordinate Initial { get; set; }
    public Coordinate Destination { get; set; }

    protected bool IsValidMove();
}