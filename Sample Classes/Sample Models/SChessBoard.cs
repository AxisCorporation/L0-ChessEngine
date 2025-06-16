using L_0_Chess_Engine.Contracts;
using L_0_Chess_Engine.Models;

namespace L_0_Chess_Engine.SampleModels;
public class SChessBoard : IChessBoard
{
    public IChessPiece[,] Grid { get; set; } = new ChessPiece[8, 8];

    public SChessBoard()
    {
        Grid[0, 0] = new ChessPiece(PieceType.Pawn | PieceType.White);
    }

    public bool IsCheck { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool IsCheckMate { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void MakeMove(IMove move)
    {
        throw new System.NotImplementedException();
    }

    public void PrintBoardToTerminal()
    {
        throw new System.NotImplementedException();
    }

    public bool ReadFEN(int[,] board, string fen)
    {
        throw new System.NotImplementedException();
    }

    public void ResetBoard()
    {
        throw new System.NotImplementedException();
    }
}