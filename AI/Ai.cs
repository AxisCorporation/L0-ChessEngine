using System;
using System.Collections.Generic;
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

            Move bestMove = null;
            int bestScore = _white ? -1000 : 1000;

            foreach (var move in moves)
            {
                if (move.InitPiece.IsWhite) continue;

                int eval = 0;

                // Simulating move
                (int initX, int initY) = move.InitPiece.Coordinates;
                (int destX, int destY) = move.DestPiece.Coordinates;

                ChessPiece originalInit = Grid[initX, initY];
                ChessPiece originalDest = Grid[destX, destY];

                // Applying move
                Grid[destX, destY] = originalInit;
                Grid[initX, initY] = new ChessPiece(PieceType.Empty, new(initX, initY));
                originalInit.Coordinates = new(destX, destY);

                switch (_difficulty)
                {
                    default:
                    case AIDifficulty.Easy:
                        eval = MiniMax(1, -1000, 1000, _white);
                        break;

                    case AIDifficulty.Medium:
                        eval = MiniMax(4, -1000, 1000, _white);
                        break;

                    case AIDifficulty.Hard:
                        eval = MiniMax(8, -1000, 1000, _white);
                        break;

                }

                // Undoing move
                Grid[initX, initY] = originalInit;
                Grid[destX, destY] = originalDest;
                originalInit.Coordinates = new(initX, initY);
                originalDest.Coordinates = new(destX, destY);

                if (_white && eval > bestScore)
                {
                    bestScore = eval;
                    bestMove = move;
                }
                else if (!_white && eval < bestScore)
                {
                    bestScore = eval;
                    bestMove = move;
                    Console.WriteLine($"AI - Type: {bestMove.InitPiece.Type} | To: {bestMove.DestPiece.Coordinates}");
                }

            }

            return bestMove;
        }

        // Function only here for hot fix, will optimize later
        public List<Move> GenerateAllMoves()
        {
            List<Move> moves = [];

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (Grid[x, y] == PieceType.Empty)
                    {
                        continue;
                    }

                    moves.AddRange(Move.GeneratePieceMoves(Grid[x, y]));
                }
            }

            return moves;
        }

        
        // This is Sebastian's implementation almost line by line, So it's unlikely that this function is wrong.
        // But we might have to cross check anyways.
        private int MiniMax(int depth, int alpha, int beta, bool maximize)
        {
            if (depth == 0 || ChessBoard.Instance.IsCheckMate)
            {
                return EvaluateBoardPosition();
            }
            
            int eval;
            
            List<Move> moves = GenerateAllMoves();

            if (maximize)
            {
                int maxEval = -1000;

                foreach (var move in moves)
                {
                    if (!move.InitPiece.IsWhite)
                    {
                        continue;
                    }

                    // ChessBoard.Instance.HypotheticalMove(move); Will work this when the function works

                    // Simulating move
                    (int initX, int initY) = move.InitPiece.Coordinates;
                    (int destX, int destY) = move.DestPiece.Coordinates;

                    ChessPiece originalInit = Grid[initX, initY];
                    ChessPiece originalDest = Grid[destX, destY];

                    // Applying move
                    Grid[destX, destY] = originalInit;
                    Grid[initX, initY] = new ChessPiece(PieceType.Empty, new(initX, initY));
                    originalInit.Coordinates = new(destX, destY);

                    eval = MiniMax(depth - 1, alpha, beta, false);

                    // ChessBoard.Instance.UndoHypotheticalMove(move);

                    // Undoing move
                    Grid[initX, initY] = originalInit;
                    Grid[destX, destY] = originalDest;
                    originalInit.Coordinates = new(initX, initY);
                    originalDest.Coordinates = new(destX, destY);

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
                int minEval = 1000;

                foreach (var move in moves)
                {
                    if (move.InitPiece.IsWhite)
                    {
                        continue;
                    }

                    // ChessBoard.Instance.HypotheticalMove(move); Will work this when the function works

                    // Simulating move
                    (int initX, int initY) = move.InitPiece.Coordinates;
                    (int destX, int destY) = move.DestPiece.Coordinates;

                    ChessPiece originalInit = Grid[initX, initY];
                    ChessPiece originalDest = Grid[destX, destY];

                    // Applying move
                    Grid[destX, destY] = originalInit;
                    Grid[initX, initY] = new ChessPiece(PieceType.Empty, new(initX, initY));
                    originalInit.Coordinates = new(destX, destY);

                    eval = MiniMax(depth - 1, alpha, beta, true);

                    // ChessBoard.Instance.UndoHypotheticalMove(move);

                    // Undoing move
                    Grid[initX, initY] = originalInit;
                    Grid[destX, destY] = originalDest;
                    originalInit.Coordinates = new(initX, initY);
                    originalDest.Coordinates = new(destX, destY);

                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(alpha, eval);
                    
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