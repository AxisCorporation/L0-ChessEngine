using L_0_Chess_Engine.Contracts;

namespace L_0_Chess_Engine.Models;
public class ChessBoard : IChessBoard
{
    public IChessPiece[,] Grid { get; set; }
    public bool IsCheck { get; set; }
    public bool IsCheckMate { get; set; }
    public ChessBoard() //constructer to initialize
    {
        Grid = new IChessPiece[8, 8];
        IsCheck = false;
        IsCheckMate = false;
        ResetBoard(); // Initialize the board
    }

    public void MakeMove(IMove move);
    public void ResetBoard();
    protected void PrintBoardToTerminal();
    protected bool ReadFEN(int[,] board, string fen);
}
