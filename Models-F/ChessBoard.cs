using L_0_Chess_Engine.Models;
using L_0_Chess_Engine.Contracts;

namespace L_0_Chess_Engine.Fake
{
    public class ChessBoard : IChessBoard
    {
        public IChessPiece[,] Grid { get; set; } = new ChessPiece[8, 8];

        public bool IsCheck { get; set; }
        public bool IsCheckMate { get; set; }

        public ChessBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Grid[i, j] = new ChessPiece();
                }
            }
        }

        public void PrintBoardToTerminal()
        {
            
        }

        public void MakeMove(IMove move)
        {

        }

        public void ResetBoard()
        {

        }

        public bool ReadFEN(int[,] grid, string fen)
        {
            return false;
        }
        
    }
    
}
