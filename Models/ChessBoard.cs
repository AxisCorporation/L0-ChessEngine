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
    public void ResetBoard()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Grid[i, j] = new ChessPiece(PieceType.Empty);
            }
        }

        // White pieces first
        Grid[0, 0] = new ChessPiece(PieceType.White | PieceType.Rook);
        Grid[0, 1] = new ChessPiece(PieceType.White | PieceType.Knight);
        Grid[0, 2] = new ChessPiece(PieceType.White | PieceType.Bishop);
        Grid[0, 3] = new ChessPiece(PieceType.White | PieceType.Queen);
        Grid[0, 4] = new ChessPiece(PieceType.White | PieceType.King);
        Grid[0, 5] = new ChessPiece(PieceType.White | PieceType.Bishop);
        Grid[0, 6] = new ChessPiece(PieceType.White | PieceType.Knight);
        Grid[0, 7] = new ChessPiece(PieceType.White | PieceType.Rook);

        for (int i = 0; i < 8; i++)
        {
            Grid[1, i] = new ChessPiece(PieceType.White | PieceType.Pawn);
        }

        // Black pieces
        Grid[7, 0] = new ChessPiece(PieceType.Black | PieceType.Rook);
        Grid[7, 1] = new ChessPiece(PieceType.Black | PieceType.Knight);
        Grid[7, 2] = new ChessPiece(PieceType.Black | PieceType.Bishop);
        Grid[7, 3] = new ChessPiece(PieceType.Black | PieceType.Queen);
        Grid[7, 4] = new ChessPiece(PieceType.Black | PieceType.King);
        Grid[7, 5] = new ChessPiece(PieceType.Black | PieceType.Bishop);
        Grid[7, 6] = new ChessPiece(PieceType.Black | PieceType.Knight);
        Grid[7, 7] = new ChessPiece(PieceType.Black | PieceType.Rook);

        for (int i = 0; i < 8; i++)
        {
            Grid[6, i] = new ChessPiece(PieceType.Black | PieceType.Pawn);
        }
    }
    protected void PrintBoardToTerminal();
    protected bool ReadFEN(int[,] board, string fen);
}
