using System;
using System.Linq;
using System.Collections.Generic;

namespace L_0_Chess_Engine.Models;

public class ChessBoard
{
    public ChessPiece[,] Grid { get; set; }
    public bool IsCheck { get => CheckScan(); }
    public bool IsCheckMate { get; set; }

    //Constant FEN for the starting position
    private const string DefaultFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

    // Invoked every time grid updates
    public event Action? GridUpdated;

    public bool IsWhiteTurn { get; set; }


    private static ChessBoard? _instance;
    public static ChessBoard Instance
    {
        get => _instance ??= new ChessBoard();
    }

    private ChessBoard() // Constructor to initialize
    {
        IsWhiteTurn = true;

        Grid = new ChessPiece[8, 8];
        IsCheckMate = false;

        ResetBoard(); // Initialize the board
    }

    public void MakeMove(Move move)
    {
        // Reset all valid En Passant moves
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Grid[i, j].IsValidPassantPlacement = false;
            }
        }

        (int initX, int initY) = move.InitPiece.Coordinates;
        (int destX, int destY) = move.DestPiece.Coordinates;

        ChessPiece pieceToMove = Grid[initX, initY];

        pieceToMove.HasMoved = true;

        if (move.InitPiece.EqualsUncolored(PieceType.Pawn))
        {
            CheckSpecialPawnConditions(move, ref pieceToMove);
        }

        Grid[initX, initY] = new ChessPiece(PieceType.Empty, new(initX, initY));
        Grid[destX, destY] = pieceToMove;
        pieceToMove.Coordinates = new(destX, destY);

        IsWhiteTurn = !IsWhiteTurn;

        GridUpdated?.Invoke();
    }

    public void ResetBoard()
    {
        ReadFEN(DefaultFEN);
    }


    // After making a move, it checks if the opposite team is in check
    private bool CheckScan()
    {
        PieceType oppositeColour;

        if (IsWhiteTurn)
        {
            oppositeColour = PieceType.Black;
        }
        else
        {
            oppositeColour = PieceType.White;
        }


        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (Grid[x, y].Type == PieceType.Empty)
                {
                    continue;
                }

                if (Grid[x, y].Color == oppositeColour)
                {
                    List<Move> moves = [..from m in Move.GetPossibleMoves(Grid[x, y])
                                       where m.TargetsKing
                                       select m]; // Gets all moves for piece where it can threaten the king

                    if (moves.Count != 0)
                    {
                        return true;
                    }
                }

            }
        }

        return false;
    }

    private void CheckSpecialPawnConditions(Move move, ref ChessPiece pieceToMove)
    {
        (int initX, int initY) = move.InitPiece.Coordinates;
        (int destX, int destY) = move.DestPiece.Coordinates;

        if (destY == 0 || destY == 7)
        {
            pieceToMove = GetPieceFromPromotion();
        }
        else if (Math.Abs(destY - initY) == 2)
        {
            int PassantY = move.InitPiece.IsWhite ? destY - 1 : destY + 1;
            Grid[destX, PassantY].IsValidPassantPlacement = true;
        }
        else if (move.IsEnPassant)
        {
            int CapturedPawnY = pieceToMove.IsWhite ? destY - 1 : destY + 1;
            Grid[destX, CapturedPawnY] = new ChessPiece(PieceType.Empty, new(destX, CapturedPawnY));
        }
    }

    public bool ReadFEN(string fen)
    {

        // Split the layout into rows (8 rows for an 8x8 board)
        var rows = fen.Split('/');
        if (rows.Length != 8)
            return false; // 8 rows must or invalid

        // Iterate through each row and parse it
        for (int y = 7; y >= 0; y--)
        {

            var row = rows[7 - y];
            int currentCol = 0;

            foreach (var c in row)
            {
                if (char.IsDigit(c))
                {
                    // Character that are numbers show empty squares
                    int emptySquares = int.Parse(c.ToString());

                    // Add empty squares to the current row
                    for (int j = 0; j < emptySquares; j++)
                    {
                        Grid[currentCol, y] = new ChessPiece(PieceType.Empty, new(currentCol, y));
                        currentCol++;
                    }
                }
                else
                {
                    // Switch statement to handle piece types
                    PieceType? pieceType = c switch
                    {
                        'p' => PieceType.Pawn | PieceType.Black,
                        'P' => PieceType.Pawn | PieceType.White,

                        'r' => PieceType.Rook | PieceType.Black,
                        'R' => PieceType.Rook | PieceType.White,

                        'n' => PieceType.Knight | PieceType.Black,
                        'N' => PieceType.Knight | PieceType.White,

                        'b' => PieceType.Bishop | PieceType.Black,
                        'B' => PieceType.Bishop | PieceType.White,

                        'q' => PieceType.Queen | PieceType.Black,
                        'Q' => PieceType.Queen | PieceType.White,

                        'k' => PieceType.King | PieceType.Black,
                        'K' => PieceType.King | PieceType.White,

                        // Invalid piece character
                        _ => null
                    };

                    if (pieceType is null)
                    {
                        return false;
                    }

                    // Create the chess piece with the correct color and type
                    Grid[currentCol, y] = new ChessPiece(pieceType.Value, new(currentCol, y));
                    currentCol++;
                }
            }
        }

        // Return true when the board is successfully set up
        return true;
    }

    public static ChessPiece GetPieceFromPromotion()
    {
        throw new NotSupportedException();
    }
}
