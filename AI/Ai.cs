using System;
using System.Collections.Generic;
using System.Linq;
using L_0_Chess_Engine.Enums;
using L_0_Chess_Engine.Models;

namespace L_0_Chess_Engine.AI
{
    class Ai
    {
        
        // Creating a copy of pointer for clarity
        ChessPiece[,] Grid = ChessBoard.Instance.Grid;

        // Used for assigning a value to each piece
        private readonly Dictionary<PieceType, int> ValueMap = [];

        private bool _white;
        private AIDifficulty _difficulty;

        public Ai(bool isWhite, AIDifficulty Difficulty)
        {
            _white = isWhite;
            _difficulty = Difficulty;

            // TODO: Initialize ValueMap with a respective value for each PieceType
            ValueMap[PieceType.Pawn] = 1;
            ValueMap[PieceType.Knight] = 3;
            ValueMap[PieceType.Bishop] = 3;
            ValueMap[PieceType.Rook] = 5;
            ValueMap[PieceType.Queen] = 9;
            ValueMap[PieceType.King] = 100; // Will Probably Change this Later
        }

        public Move GenerateMove()
        {
            List<Move> moves = GenerateAllMoves();

            List<Move> bestMoves = new();
            int bestScore = _white ? int.MinValue : int.MaxValue;

            Random rng = new Random();

            foreach (var move in moves)
            {
                if (move.InitPiece.IsWhite != _white) continue;

                int eval = 0;

                ChessBoard.Instance.HypotheticalMove(move);

                eval = _difficulty switch
                {
                    AIDifficulty.Easy => MiniMax(1, int.MinValue, int.MaxValue, !_white),
                    AIDifficulty.Medium => MiniMax(4, int.MinValue, int.MaxValue, !_white),
                    AIDifficulty.Hard => MiniMax(8, int.MinValue, int.MaxValue, !_white),
                    _ => MiniMax(2, int.MinValue, int.MaxValue, !_white)
                };


                ChessBoard.Instance.UndoHypotheticalMove(move);

                if (_white)
                {
                    if (eval > bestScore)
                    {
                        bestScore = eval;
                        bestMoves.Clear();
                        bestMoves.Add(move);
                    }
                    else if (eval == bestScore)
                    {
                        bestMoves.Add(move);
                    }
                }
                else
                {
                    if (eval < bestScore)
                    {
                        bestScore = eval;
                        bestMoves.Clear();
                        bestMoves.Add(move);
                    }
                    else if (eval == bestScore)
                    {
                        bestMoves.Add(move);
                    }
                }

            }

            return bestMoves.Count > 0 ? bestMoves[rng.Next(bestMoves.Count)] : null;;
        }

        public List<Move> GenerateAllMoves()
        {
            List<Move> moves = [];

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (Grid[x, y].Type == PieceType.Empty)
                    {
                        continue;
                    }

                    moves.AddRange(Move.GeneratePieceMoves(Grid[x, y]));
                }
            }

            return moves;
        }

        private List<Move> OrderMoves(List<Move> moves)
        {
            // TODO

            return moves.OrderByDescending(EvaluateMove).ToList();
        }

        private int EvaluateMove(Move move)
        {
            int score = 0;

            PieceType initType = move.InitPiece.Type & ~PieceType.White & ~PieceType.Black;
            int initValue = ValueMap.TryGetValue(initType, out var iv) ? iv : 0;

            PieceType destType = move.IsEnPassant ? PieceType.Pawn : move.DestPiece.Type & ~PieceType.White & ~PieceType.Black;
            int destValue = ValueMap.TryGetValue(initType, out var dv) ? dv : 0;

            (int destX, int destY) = move.DestPiece.Coordinates;

            if (destType != PieceType.Empty)
            {
                score += 1000 + 100 * destValue - 10 * initValue;
            }

            bool isPromotion = move.IsPromotion;
            PieceType promotionBase = move.PromotionPiece & ~PieceType.White & ~PieceType.Black;

            if (isPromotion)
            {
                int promoVal = ValueMap.TryGetValue(promotionBase, out var pv) ? pv : 0;
                score += 800 + 10 * promoVal;
            }

            int toCenter = Math.Min(Math.Abs(destX - 3), Math.Abs(destX - 4)) + Math.Min(Math.Abs(destY - 3), Math.Abs(destY - 4));

            score += 4 - toCenter;

            if (initType == PieceType.Pawn) score += 1;

            return score;
        }

        
        // This is Sebastian's implementation almost line by line, So it's unlikely that this function is wrong.
        // But we might have to cross check anyways.
        private int MiniMax(int depth, int alpha, int beta, bool maximize)
        {
            if (depth == 0 || ChessBoard.Instance.IsCheckMate)
            {
                return EvaluateBoardPosition();
            }

            var sideMoves = new List<Move>();
            foreach (var m in GenerateAllMoves())
                if (m.InitPiece.IsWhite == maximize) sideMoves.Add(m);

            List<Move> moves = OrderMoves(sideMoves);

            int eval;

            if (maximize)
            {
                int maxEval = int.MinValue;

                foreach (var move in moves)
                {

                    ChessBoard.Instance.HypotheticalMove(move);

                    eval = MiniMax(depth - 1, alpha, beta, false);

                    ChessBoard.Instance.UndoHypotheticalMove(move);

                    maxEval = Math.Max(maxEval, eval);

                    alpha = Math.Max(alpha, eval);

                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;

                foreach (var move in moves)
                {

                    ChessBoard.Instance.HypotheticalMove(move);

                    eval = MiniMax(depth - 1, alpha, beta, true);

                    ChessBoard.Instance.UndoHypotheticalMove(move);

                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);

                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return minEval;
            }

        }


        private int EvaluateBoardPosition()
        {
            int score = 0;

            // Gotta evaluate the board after move
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    ChessPiece piece = Grid[x, y];
                    if (piece.Type == PieceType.Empty) continue;

                    PieceType baseType = piece.Type & ~PieceType.White & ~PieceType.Black;

                    if (!ValueMap.TryGetValue(baseType, out int value))
                        continue;

                    int positionValue = 0;
                    // Bonus for controlling center
                    if ((x == 3 || x == 4) && (y == 3 || y == 4))
                    {
                        positionValue += 10; 
                    }
                    
                    // Bonus for advancing pawns 
                    if (baseType == PieceType.Pawn)
                    {
                        if (piece.IsWhite)
                        {
                            positionValue += y * 5;
                        }
                        else
                        {
                            positionValue += (7 - y) * 5;
                        }
                    }

                    // Knight penalty for edge positions
                    if (baseType == PieceType.Knight)
                    {
                        if (x == 0 || x == 7 || y == 0 || y == 7)
                        {
                            positionValue -= 10; 
                        }
                    }

                    if (piece.IsWhite)
                        score += value;
                    else
                        score -= value;
                }
            }

            return score; // Lower is better for bot
        }
    }
    
}