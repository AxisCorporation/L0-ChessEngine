
// We will have to change the structure here later, bit for now this is fine.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using L_0_Chess_Engine.Models;

namespace L_0_Chess_Engine.AI
{
    class Ai
    {
        private static readonly Dictionary<PieceType, int> ValueMap = [];

        static Ai()
        {
            ValueMap[PieceType.Pawn] = 1;
            ValueMap[PieceType.Knight] = 3;
            ValueMap[PieceType.Bishop] = 3;
            ValueMap[PieceType.Rook] = 5;
            ValueMap[PieceType.Queen] = 9;

            ValueMap[PieceType.King] = 0;
        }
        public async Task<Move> GenerateMove()
        {
            var moves = new List<Move> { };

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (ChessBoard.Instance.Grid[x, y].IsWhite || ChessBoard.Instance.Grid[x, y] == PieceType.Empty)
                    {
                        continue;
                    }

                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            Move move = new(ChessBoard.Instance.Grid[x, y], ChessBoard.Instance.Grid[i, j]);

                            if (move.IsValid) moves.Add(move);
                        }
                    }

                }
            }

            var random = new Random();
            int index = random.Next(moves.Count); // from 0 to moves.Count - 1

            await Task.Delay(2000);
            return moves[index];
        }


        private int EvaluateBoard(ChessPiece[,] Grid)
        {
            int whiteScore = 0;
            int blackScore = 0;

            foreach (var square in Grid)
            {
                
            }
        }
    }
}