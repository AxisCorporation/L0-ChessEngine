
// We will have to change the structure here later, bit for now this is fine.
using L_0_Chess_Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace L_0_Chess_Engine.AI
{
    class Ai
    {
        private readonly Random _random = new();
        private Move? _lastMove;

        public Move GenerateMove()
        {
            var board = ChessBoard.Instance;
            var moves = new List<Move>();

            //Collect all valid black moves
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var piece = board.Grid[x, y];
                    if (piece.Type == PieceType.Empty || piece.IsWhite)
                        continue;

                    foreach (var move in Move.GetPossibleMoves(piece))
                    {
                        if (!move.IsValid || board.WouldCauseCheck(move))
                            continue;

                        moves.Add(move);
                    }
                }
            }

            //Prioritize pawns
            var pawnMoves = moves.Where(m => m.InitPiece.EqualsUncolored(PieceType.Pawn)).ToList();

            // Only pawns if possible
            if (pawnMoves.Count > 0)
                moves = pawnMoves;

            // Repeat previous move backwards
            if (_lastMove != null)
            {
                var backMove = moves.FirstOrDefault(m =>
                    m.InitPiece == _lastMove.DestPiece &&
                    m.DestPiece.Coordinates.Equals(_lastMove.InitPiece.Coordinates));

                if (backMove != null)
                {
                    _lastMove = backMove;
                    return backMove;
                }
            }

            //Avoid captures / checkmates
            var safeMoves = moves.Where(m => m.DestPiece == PieceType.Empty && !m.TargetsKing).ToList();
            if (safeMoves.Count > 0)
                moves = safeMoves;

            //Random among remaining
            var selected = moves[_random.Next(moves.Count)];
            _lastMove = selected;
            return selected;
        }
    }

}