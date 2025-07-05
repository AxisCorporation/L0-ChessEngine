using System;
using System.Drawing;
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

    // Temp Variable
    private bool _isWhiteTurn;

    private PieceType CheckColour
    {
        get
        {
            if (_isWhiteTurn) return PieceType.White;

            return PieceType.Black;
        }
    }


    private static ChessBoard? _instance;
    public static ChessBoard Instance
    {
        get => _instance ??= new ChessBoard();
    }

    private ChessBoard() // Constructor to initialize
    {
        _isWhiteTurn = true;

        Grid = new ChessPiece[8, 8];
        IsCheck = false;
        IsCheckMate = false;

        ResetBoard(); // Initialize the board
    }

    public void MakeMove(IMove moveInterface)
    {
        Move move = (Move) moveInterface;

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


        ChessPiece pieceToMove = move.InitPiece;
        pieceToMove.HasMoved = true;

        ChessPiece PieceToMove = (ChessPiece) Grid[initX, initY];
        PieceToMove.HasMoved = true;

        if (move.InitPiece.EqualsUncolored(PieceType.Pawn))
        {
            CheckSpecialPawnConditions(move, ref pieceToMove);
        }

        Grid[initX, initY] = new ChessPiece(PieceType.Empty, new(initX, initY));
        Grid[destX, destY] = pieceToMove;

        pieceToMove.Coordinates = move.DestPiece.Coordinates;

        // Check Logic
        _isWhiteTurn = !_isWhiteTurn;
        CheckScan(CheckColour);

        Console.WriteLine($"{CheckColour} : {IsCheck}");

        GridUpdated?.Invoke();
    }

    public void ResetBoard()
    {
        ReadFEN(DefaultFEN);
    }


    private void CheckScan(PieceType colour)
    {
        PieceType oppositeColour = (PieceType)((int)colour ^ 0b11_000);

        int kingX = 0, kingY = 0;

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (Grid[x, y].Type == (PieceType.King | colour))
                {
                    (kingX, kingY) = (x, y);
                    break;
                }
            }
        }

        for (int Y = 0; Y < 8; Y++)
        {
            for (int X = 0; X < 8; X++)
            {
                if ((int)(Grid[X, Y].Type & oppositeColour) != 0)
                {
                    Move move = new((ChessPiece)Grid[X, Y], (ChessPiece)Grid[kingX, kingY]);

                    if (move.IsValid)
                    {
                        IsCheck = true;
                        return;
                    }
                    
                }
            }
        }

        IsCheck = false;

    }

    private void CheckSpecialPawnConditions(Move move, ref ChessPiece PieceToMove)
    {
        (int initX, int initY) = move.InitPiece.Coordinates;
        (int destX, int destY) = move.DestPiece.Coordinates;

        if (destY == 0 || destY == 7)
        {
            PieceToMove = GameManager.GetPieceFromPromotion();
        }
        else if (Math.Abs(destY - initY) == 2)
        {
            int PassantY = move.InitPiece.IsWhite ? destY - 1 : destY + 1;
            Grid[destX, PassantY].IsValidPassantPlacement = true;
        }
        else if (move.IsEnPassant)
        {
            int CapturedPawnY = PieceToMove.IsWhite ? destY - 1 : destY + 1;
            Grid[destX, CapturedPawnY] = new ChessPiece(PieceType.Empty, new(CapturedPawnY, destX));
        }
    }

    public void PrintBoardToTerminal()
    {

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
}
