using L_0_Chess_Engine.Models;

namespace L_0_Chess_Engine.Contracts;

public record Coordinate(int X, int Y);

public interface IChessPiece
{
    public PieceType Type { get; set; }
    public bool IsWhite { get; }
}

public interface IChessBoard
{
    public IChessPiece[,] Grid { get; set; }
    public bool IsCheck { get; set; }
    public bool IsCheckMate { get; set; }

    public void MakeMove(IMove move);
    public void ResetBoard();
    protected void PrintBoardToTerminal();

    protected bool ReadFEN(int[,] board, string fen);
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

    public IChessPiece Piece { get; set; }
    public Coordinate Initial { get; set; }
    public Coordinate Destination { get; set; }

    protected bool IsValidMove();
}