using L_0_Chess_Engine.Contracts;
using System;

namespace L_0_Chess_Engine.Models;

public class ChessBoard : IChessBoard
{
    public IChessPiece[,] Grid { get; set; }
    public bool IsCheck { get; set; }
    public bool IsCheckMate { get; set; }

    //Constant FEN for the starting position
    private const string DefaultFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

    public ChessBoard() //constructer to initialize
    {
        // I have already implemented the Chess Piece Class, so you didn't need to initialize it to the Interface anymore
        // This Initializes every piece to a White Pawn but that won't be a problem with the Reset Board function
        Grid = new ChessPiece[8, 8];
        IsCheck = false;
        IsCheckMate = false;

        ResetBoard(); // Initialize the board
    }

    public void MakeMove(IMove move)
    {
        // Zain here, it's better to leave functions that you haven't implemented yet empty rather than
        // just leaving their function signature, I mean doesn't all the red error lines annoy you?
    }

    public void ResetBoard()
    {
        ReadFEN(DefaultFEN);
    }

    public void PrintBoardToTerminal()
    {
        // Also this functions should be "public" not "protected"
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
                if (Char.IsDigit(c))
                {
                    // Character that are numbers show empty squares
                    int emptySquares = int.Parse(c.ToString());

                    // Add empty squares to the current row
                    for (int j = 0; j < emptySquares; j++)
                    {
                        Grid[i, currentCol++] = new ChessPiece(PieceType.Empty);
                    }
                }
                else
                {
                    PieceType pieceType = PieceType.Empty;

                    // Switch statement to handle piece types
                    switch (c)
                    {
                        case 'p':
                            pieceType = PieceType.Pawn | PieceType.Black;
                            break;
                        case 'P':
                            pieceType = PieceType.Pawn | PieceType.White;
                            break;
                        case 'r':
                            pieceType = PieceType.Rook | PieceType.Black;
                            break;
                        case 'R':
                            pieceType = PieceType.Rook | PieceType.White;
                            break;
                        case 'n':
                            pieceType = PieceType.Knight | PieceType.Black;
                            break;
                        case 'N':
                            pieceType = PieceType.Knight | PieceType.White;
                            break;
                        case 'b':
                            pieceType = PieceType.Bishop | PieceType.Black;
                            break;
                        case 'B':
                            pieceType = PieceType.Bishop | PieceType.White;
                            break;
                        case 'q':
                            pieceType = PieceType.Queen | PieceType.Black;
                            break;
                        case 'Q':
                            pieceType = PieceType.Queen | PieceType.White;
                            break;
                        case 'k':
                            pieceType = PieceType.King | PieceType.Black;
                            break;
                        case 'K':
                            pieceType = PieceType.King | PieceType.White;
                            break;
                        default:
                            return false;  // Invalid piece character
                    }

                    // Create the chess piece with the correct color and type
                    Grid[i, currentCol++] = new ChessPiece(pieceType);
                }
            }
        }

        // Return true when the board is successfully set up
        return true;
    }
}
