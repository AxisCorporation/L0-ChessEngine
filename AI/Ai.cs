
// We will have to change the structure here later, bit for now this is fine.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using L_0_Chess_Engine.Models;
using System.Linq;
using System.Diagnostics;

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
            List<Move> moves = [];

            var GenerateAllMoves = Task.Run(() =>
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        if (ChessBoard.Instance.Grid[x, y] == PieceType.Empty || ChessBoard.Instance.Grid[x, y].IsWhite)
                        {
                            continue;
                        }

                        moves.AddRange(Move.GetPossibleMoves(ChessBoard.Instance.Grid[x, y]));
                    }
                }
            });

            await GenerateAllMoves;

            var moveEvals = from m in moves
                            select new
                            {
                                StoredMove = m,
                                Eval = EvaluateMove(ChessBoard.Instance.Grid, m)
                            };

            foreach (var move in moveEvals)
            {
                Debug.WriteLine("DEBUG: ");
                Debug.WriteLine($"Move: {move.StoredMove.InitPiece.Type} | {move.StoredMove.InitPiece.Coordinates} | {move.StoredMove.DestPiece.Coordinates}");
                Debug.WriteLine($"Eval: {move.Eval}");
            }

            var maxEval = moveEvals.Min(x => x.Eval); // min is better for

            List<Move> bestMoves = [..from m in moveEvals
                                    where m.Eval == maxEval
                                    select m.StoredMove];

            Random rand = new();
            int random = rand.Next(bestMoves.Count);

            await Task.Delay(2000);
            return bestMoves[random];
        }


        private int EvaluateMove(ChessPiece[,] Grid, Move move)
        {
            int whiteScore = 0;
            int blackScore = 0;

            (int initX, int initY) = move.InitPiece.Coordinates;
            (int destX, int destY) = move.DestPiece.Coordinates;

            ChessPiece pieceToMove = Grid[initX, initY];
            ChessPiece destPiece = Grid[destX, destY];

            Grid[destX, destY] = pieceToMove;
            bool hasMovedPreviously = pieceToMove.HasMoved;
            pieceToMove.HasMoved = true;

            pieceToMove.Coordinates = new(destX, destY);

            Grid[initX, initY] = new(PieceType.Empty, new(initX, initY));

            foreach (var square in Grid)
            {
                if (square == PieceType.Empty)
                {
                    continue;
                }

                PieceType typeUncolored = square.Type ^ square.Color;

                if (square.IsWhite)
                {
                    whiteScore += ValueMap[typeUncolored];
                }
                else
                {
                    blackScore += ValueMap[typeUncolored];
                }
            }

            if (ChessBoard.Instance.IsCheck && !ChessBoard.Instance.IsWhiteTurn)
            {
                Console.WriteLine("Here");
                Grid[initX, initY] = pieceToMove;
                Grid[destX, destY] = destPiece;

                pieceToMove.Coordinates = new(initX, initY);
                pieceToMove.HasMoved = hasMovedPreviously;

                return -36;
            }
            else if (ChessBoard.Instance.IsCheck)
            {
                blackScore -= 36;
            }

            // revert move
            Grid[initX, initY] = pieceToMove;
            Grid[destX, destY] = destPiece;

            pieceToMove.Coordinates = new(initX, initY);
            pieceToMove.HasMoved = hasMovedPreviously;

            return whiteScore - blackScore; // Lower value is better for the AI
        }
    }
}