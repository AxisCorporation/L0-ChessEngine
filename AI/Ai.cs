
using System.Collections.Generic;
using L_0_Chess_Engine.Models;

namespace L_0_Chess_Engine.AI
{
    class Ai
    {
        bool maximizePlayer = false;
        private readonly Dictionary<PieceType, int> ValueMap;

        public Ai(bool maxPlayer)
        {
            maximizePlayer = maxPlayer;

            // Initialize ValueMap with a respective value for each PieceType
        }

        public Move GenerateMove()
        {
            var moves = new List<Move> { };

            // Creating a copy of pointer for clarity
            ChessPiece[,] Grid = ChessBoard.Instance.Grid;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (Grid[x, y].IsWhite || Grid[x, y] == PieceType.Empty)
                    {
                        continue;
                    }

                    moves.AddRange(Move.GetPossibleMoves(Grid[x, y]));
                }
            }

            return MiniMaxMove(moves);
        }

        private Move MiniMaxMove(List<Move> moves)
        {
            // Recursive logic to determine the highest and lowest move score
            // Return lowest scoring move if maximizePlayer = true
            return moves[0];
        }

        private int EvaluateMove(Move move)
        {
            int whiteScore = 0;
            int blackScore = 0;

            // Calculate each side's score based on piece value
            blackScore += EvaluateBoardPosition();

            return whiteScore - blackScore; // Lower is better for bot
        }

        private int EvaluateBoardPosition()
        {
            // Write an algorithm to determine how good the board position (not counting piece value) is for the bot
            return 0; 
        }
    }
    
}