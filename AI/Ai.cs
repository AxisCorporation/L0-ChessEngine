
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
        private readonly Dictionary<PieceType, int> ValueMap = [];

        bool maximizePlayer = false;

        public Ai(bool maxPlayer)
        {
            maximizePlayer = maxPlayer;

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

            return MiniMaxMove(0, moves);
        }

        // Function only here for hot fix, will optimize later
        public List<Move> TempGenerateMove()
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

                if (bestMove == null)
                {
                    bestMove = moves[0];
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
                    // ChessBoard.Instance.MakeMove(move);

                    (int initX, int initY) = move.InitPiece.Coordinates;
                    (int destX, int destY) = move.DestPiece.Coordinates;

                    bestMoves.Add(MiniMaxMove(depth - 1, GenerateMoves()));
                    ChessPiece originalInit = Grid[initX, initY];
                    ChessPiece originalDest = Grid[destX, destY];

                    // Applying move
                    Grid[destX, destY] = originalInit;
                    Grid[initX, initY] = new ChessPiece(PieceType.Empty, new(initX, initY));
                    originalInit.Coordinates = new(destX, destY);

                    bestMoves.Add(MiniMaxMove(depth - 1, TempGenerateMove()));

                    // Undoing move
                    Grid[initX, initY] = originalInit;
                    Grid[destX, destY] = originalDest;
                    originalInit.Coordinates = new(initX, initY);
                    originalDest.Coordinates = new(destX, destY);

                }

            }
            // Min
            else
            {
                foreach (var move in moves)
                {
                    // Will change this to make a "Hypothetical Move"
                    // ChessBoard.Instance.MakeMove(move);

                    (int initX, int initY) = move.InitPiece.Coordinates;
                    (int destX, int destY) = move.DestPiece.Coordinates;


                    bestMoves.Add(MiniMaxMove(depth - 1, GenerateMoves()));
                  
                    ChessPiece originalInit = Grid[initX, initY];
                    ChessPiece originalDest = Grid[destX, destY];

                    // Applying move
                    Grid[destX, destY] = originalInit;
                    Grid[initX, initY] = new ChessPiece(PieceType.Empty, new(initX, initY));
                    originalInit.Coordinates = new(destX, destY);

                    bestMoves.Add(MiniMaxMove(depth - 1, TempGenerateMove()));

                    // Undoing move
                    Grid[initX, initY] = originalInit;
                    Grid[destX, destY] = originalDest;
                    originalInit.Coordinates = new(initX, initY);
                    originalDest.Coordinates = new(destX, destY);
                }

            }

            return MiniMaxMove(0, bestMoves);
        }

        private int EvaluateMove(Move move)
        {
            int score = 0;

            // Simulating move
            (int initX, int initY) = move.InitPiece.Coordinates;
            (int destX, int destY) = move.DestPiece.Coordinates;

            ChessPiece originalInit = Grid[initX, initY];
            ChessPiece originalDest = Grid[destX, destY];

            // Applying move
            Grid[destX, destY] = originalInit;
            Grid[initX, initY] = new ChessPiece(PieceType.Empty, new(initX, initY));
            originalInit.Coordinates = new(destX, destY);

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

            // Undoing move
            Grid[initX, initY] = originalInit;
            Grid[destX, destY] = originalDest;
            originalInit.Coordinates = new(initX, initY);
            originalDest.Coordinates = new(destX, destY);

            score -= EvaluateBoardPosition();

            return score; // Lower is better for bot
        }

        private int EvaluateBoardPosition()
        {
            // TODO: Write an algorithm to determine how good the board position (not counting piece value) is for the AI
            return 0; 
        }
    }
    
}