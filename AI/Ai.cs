
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using L_0_Chess_Engine.Models;

namespace L_0_Chess_Engine.AI
{
    class Ai
    {
        
        // Creating a copy of pointer for clarity
        ChessPiece[,] Grid = ChessBoard.Instance.Grid;

        // Used for assigning a value to each piece
        private readonly Dictionary<PieceType, int> ValueMap;

        bool maximizePlayer = false;

        public Ai(bool maxPlayer)
        {
            maximizePlayer = maxPlayer;

            // TODO: Initialize ValueMap with a respective value for each PieceType
        }

        public List<Move> GenerateMove()
        {
            List<Move> moves = [];

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

            return moves;
        }



        private Move MiniMaxMove(int depth, List<Move> moves)
        {
            // TODO: 
            // Recursive logic to determine the highest and lowest move score
            // Return lowest scoring move if maximizePlayer = true

            Move? bestMove = null;

            if (depth == 0 || ChessBoard.Instance.IsCheckMate)
            {
                int bestEval = 0;

                foreach (var move in moves)
                {
                    int eval = EvaluateMove(move);

                    if (eval < bestEval)
                    {
                        bestEval = eval;
                        bestMove = move;
                    }
                }

                return bestMove;

            }
            
            List<Move> bestMoves = new List<Move>();

            // Max
            if (ChessBoard.Instance.IsWhiteTurn)
            {
                foreach (var move in moves)
                {
                    // Will change this to make a "Hypothetical Move"
                    ChessBoard.Instance.MakeMove(move);

                    bestMoves.Add(MiniMaxMove(depth - 1, GenerateMove()));

                }

            }
            // Min
            else
            {
                foreach (var move in moves)
                {
                    // Will change this to make a "Hypothetical Move"
                    ChessBoard.Instance.MakeMove(move);

                    bestMoves.Add(MiniMaxMove(depth - 1, GenerateMove()));

                }

            }

            return MiniMaxMove(0, bestMoves);
        }

        private int EvaluateMove(Move move)
        {
            int score = 0;

            // TODO: Calculate each side's score based on piece value

            // Add board position score to black's score
            

            return score; // Lower is better for bot
        }

        private int EvaluateBoardPosition()
        {
            // TODO: Write an algorithm to determine how good the board position (not counting piece value) is for the AI
            return 0; 
        }
    }
    
}