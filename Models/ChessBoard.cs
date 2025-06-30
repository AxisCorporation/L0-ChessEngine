using System;
using L_0_Chess_Engine.Contracts;

namespace L_0_Chess_Engine.Models;

public class ChessBoard : IChessBoard
{
    public IChessPiece[,] Grid { get; set; }
    public bool IsCheck { get; set; }
    public bool IsCheckMate { get; set; }

    //Constant FEN for the starting position
    private const string DefaultFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

    // Invoked every time grid updates
    public event Action? GridUpdated; 

    public ChessBoard() // Constructor to initialize
    {
        Grid = new ChessPiece[8, 8];
        IsCheck = false;
        IsCheckMate = false;

        ResetBoard(); // Initialize the board
    }

    public void MakeMove(IMove moveInterface)
    {
        Move move = (Move) moveInterface;
        if (!move.IsValid)
        {
            return;
        }

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

        ChessPiece pieceToMove = (ChessPiece) Grid[initX - 1, initY - 1];
        pieceToMove.HasMoved = true;

        if (move.InitPiece.EqualsUncolored(PieceType.Pawn))
        {
            CheckSpecialPawnConditions(move, ref pieceToMove);
        }

        Grid[initX - 1, initY - 1] = new ChessPiece(PieceType.Empty, new(initX, initY));
        Grid[destX - 1, destY - 1] = pieceToMove;

        GridUpdated?.Invoke();
    }

    public void ResetBoard()
    {
        ReadFEN(DefaultFEN);
    }

    private void CheckSpecialPawnConditions(Move move, ref ChessPiece PieceToMove)
    {
        (int initX, int initY) = move.InitPiece.Coordinates;
        (int destX, int destY) = move.DestPiece.Coordinates;

        if (destY == 1 || destY == 8)
        {
            PieceToMove = GameManager.GetPieceFromPromotion();
        }
        else if (Math.Abs(destY - initY) == 2)
        {
            Grid[destY - 2, destX - 1].IsValidPassantPlacement = true;
        }
        else if (move.IsEnPassant)
        {
            int CapturedPawnY = PieceToMove.IsWhite ? destY - 2 : destY + 1;

            Grid[CapturedPawnY, destX - 1] = new ChessPiece(PieceType.Empty, new(CapturedPawnY + 1, destX));
        }
    }

    public void PrintBoardToTerminal()
    {

    }

    public bool ReadFEN(string fen)
    {
        // Split the FEN string by spaces
        var parts = fen.Split(' ');

        if (parts.Length == 0)
            return false;

        var boardLayout = parts[0];  // First part is the board layout

        // Split the layout into rows (8 rows for an 8x8 board)
        var rows = boardLayout.Split('/');
        if (rows.Length != 8)
            return false; // 8 rows must or invalid

        // Iterate through each row and parse it
        for (int i = 0; i < 8; i++)
        {
            var row = rows[i];
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
                        Grid[i, currentCol] = new ChessPiece(PieceType.Empty, new(i + 1, ++currentCol));
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
                    Grid[i, currentCol] = new ChessPiece(pieceType.Value, new(i + 1, ++currentCol));
                }
            }
        }

        // Return true when the board is successfully set up
        return true;
    }
}
