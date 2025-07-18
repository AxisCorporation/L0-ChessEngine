
// We will have to change the structure here later, bit for now this is fine.
using System;
using System.Collections.Generic;
using L_0_Chess_Engine.Models;

namespace L_0_Chess_Engine.AI
{
    class Ai
    {
        public Move GenerateMove()
        {
            var moves = new List<Move>{};

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

            return moves[index];
        }
        
    }
    
}