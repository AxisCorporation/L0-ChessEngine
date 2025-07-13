using Avalonia.Controls;
using L_0_Chess_Engine.Models;
using System;

namespace L_0_Chess_Engine.Tests;

public static class AllTests
{

    public static void TestReadFENValid()
    {
        var board = ChessBoard.Instance;
        bool result = board.ReadFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");

        if (!result)
            throw new Exception("TestReadFENValid failed since ReadFEN returned false");

        if (board.Grid[0, 0].Type != (PieceType.Rook | PieceType.Black))
            throw new Exception("TestReadFENValid failed since top-left piece not correct");

        Console.WriteLine("TestReadFENValid passed.");
    }

    public static void TestReadFENInvalidRow()
    {
        var board = ChessBoard.Instance;
        bool result = board.ReadFEN("8/8/8/8/8/8/8"); // only 7 rows

        if (result)
            throw new Exception("TestReadFENInvalidRow failed since should return false");

        Console.WriteLine("TestReadFENInvalidRow passed.");
    }

    public static void TestReadFENInvalidCharacter()
    {
        var board = ChessBoard.Instance;

        bool result = board.ReadFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBXR"); // X is invalid

        if (result)
            throw new Exception("TestReadFENInvalidCharacter failed since should return false");

        Console.WriteLine("TestReadFENInvalidCharacter passed.");
    }

    // Helper method to find a king of specified color on the board
    private static (int x, int y) FindKing(ChessBoard board, bool isWhite)
    {
        PieceType kingType = PieceType.King | (isWhite ? PieceType.White : PieceType.Black);

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (board.Grid[x, y].Type == kingType)
                {
                    return (x, y);
                }
            }
        }

        throw new Exception($"Could not find {(isWhite ? "white" : "black")} king on the board");
    }

    // Helper method to find a piece of specified type and color on the board
    private static (int x, int y) FindPiece(ChessBoard board, PieceType type, bool isWhite)
    {
        PieceType pieceType = type | (isWhite ? PieceType.White : PieceType.Black);

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if ((board.Grid[x, y].Type & ~(PieceType.White | PieceType.Black)) == (type & ~(PieceType.White | PieceType.Black)) &&
                    board.Grid[x, y].IsWhite == isWhite)
                {
                    return (x, y);
                }
            }
        }

        throw new Exception($"Could not find {(isWhite ? "white" : "black")} {type} on the board");
    }

    // Test cases for check logic
    public static void TestCheckByQueen()
    {
        var board = ChessBoard.Instance;
        // Setup a position where white king is in check by black queen
        board.ReadFEN("8/8/8/8/8/8/4q3/4K3");

        var (queenX, queenY) = FindPiece(board, PieceType.Queen, false);

        // Moving the black queen closer to the king (still in check)
        Move move = new Move(board.Grid[queenX, queenY], board.Grid[queenX, queenY + 1]);
        board.MakeMove(move);

        if (!board.IsCheck)
            throw new Exception("TestCheckByQueen failed: King should be in check");

        Console.WriteLine("TestCheckByQueen passed.");
    }

    public static void TestCheckByRook()
    {
        var board = ChessBoard.Instance;
        // Setup a position where black king is in check by white rook
        board.ReadFEN("4k3/8/8/8/8/8/8/R3K3");

        var (kingX, kingY) = FindKing(board, true);

        Move move = new Move(board.Grid[kingX, kingY], board.Grid[kingX + 1, kingY]);
        board.MakeMove(move);

        if (!board.IsCheck)
            throw new Exception("TestCheckByRook failed: King should be in check");

        Console.WriteLine("TestCheckByRook passed.");
    }

    public static void TestCheckByBishop()
    {
        var board = ChessBoard.Instance;
        // Setup a position where white king is in check by black bishop
        board.ReadFEN("8/8/8/8/6b1/8/8/4K3");

        var (bishopX, bishopY) = FindPiece(board, PieceType.Bishop, false);

        // Moving the black bishop closer (still in check)
        Move move = new Move(board.Grid[bishopX, bishopY], board.Grid[bishopX - 1, bishopY - 1]);
        board.MakeMove(move);

        if (!board.IsCheck)
            throw new Exception("TestCheckByBishop failed: King should be in check");

        Console.WriteLine("TestCheckByBishop passed.");
    }

    public static void TestCheckByKnight()
    {
        var board = ChessBoard.Instance;
        // Setup a position where black king is in check by white knight
        board.ReadFEN("4k3/8/8/8/8/3N4/8/4K3");

        var (kingX, kingY) = FindKing(board, true);

        Move move = new Move(board.Grid[kingX, kingY], board.Grid[kingX + 1, kingY]);
        board.MakeMove(move);

        if (!board.IsCheck)
            throw new Exception("TestCheckByKnight failed: King should be in check");

        Console.WriteLine("TestCheckByKnight passed.");
    }

    public static void TestCheckByPawn()
    {
        var board = ChessBoard.Instance;
        // Setup a position where white king is in check by black pawn
        board.ReadFEN("8/8/8/8/8/3p4/4K3/8");

        var (kingX, kingY) = FindKing(board, true);

        Move move = new Move(board.Grid[kingX, kingY], board.Grid[kingX + 1, kingY]);
        board.MakeMove(move);

        if (!board.IsCheck)
            throw new Exception("TestCheckByPawn failed: King should be in check");

        Console.WriteLine("TestCheckByPawn passed.");
    }

    public static void TestNoCheck()
    {
        var board = ChessBoard.Instance;
        // Setup a position where no king is in check
        board.ReadFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");

        var (pawnX, pawnY) = FindPiece(board, PieceType.Pawn, true);

        // Make a move that doesn't result in check
        Move move = new Move(board.Grid[pawnX, pawnY], board.Grid[pawnX, pawnY + 2]);
        board.MakeMove(move);

        if (board.IsCheck)
            throw new Exception("TestNoCheck failed: No king should be in check");

        Console.WriteLine("TestNoCheck passed.");

    public static void TestCastlingKingside()
    {
        var board = ChessBoard.Instance;

        // remove everything between king and rook (kingside)
        board.Grid[5, 0] = new(PieceType.Empty, new(5, 0));
        board.Grid[6, 0] = new(PieceType.Empty, new(6, 0));

        Move move1 = new(board.Grid[4, 0], board.Grid[6, 0]);
        board.MakeMove(move1);

        if (!board.Grid[5, 0].EqualsUncolored(PieceType.Rook))
        {
            Console.WriteLine("White kingside castling failed! Rook did not move to f1 after king moved to g1.");
        }

        if (!board.Grid[6, 0].EqualsUncolored(PieceType.King))
        {
            Console.WriteLine("White kingside castling failed! King did not move to g1 after castling.");
        }

        board.ResetBoard();

        board.Grid[5, 7] = new(PieceType.Empty, new(5, 7));
        board.Grid[6, 7] = new(PieceType.Empty, new(6, 7));

        Move move2 = new(board.Grid[4, 7], board.Grid[6, 7]);
        board.MakeMove(move2);

        if (!board.Grid[5, 7].EqualsUncolored(PieceType.Rook))
        {
            Console.WriteLine("Black kingside castling failed! Rook did not move to f1 after king moved to g1.");
        }

        if (!board.Grid[6, 7].EqualsUncolored(PieceType.King))
        {
            Console.WriteLine("Black kingside castling failed! King did not move to g1 after castling.");
        }
    }

    public static void TestCastlingQueenside()
    {
        var board = ChessBoard.Instance;

        // remove everything between king and rook (queenside)
        board.Grid[1, 0] = new(PieceType.Empty, new(1, 0));
        board.Grid[2, 0] = new(PieceType.Empty, new(2, 0));
        board.Grid[3, 0] = new(PieceType.Empty, new(3, 0));

        Move move1 = new(board.Grid[4, 0], board.Grid[2, 0]);
        board.MakeMove(move1);

        if (!board.Grid[3, 0].EqualsUncolored(PieceType.Rook))
        {
            Console.WriteLine("White queenside castling failed! Rook did not move to d1 after king moved to c1.");
        }

        if (!board.Grid[2, 0].EqualsUncolored(PieceType.King))
        {
            Console.WriteLine("White queenside castling failed! King did not move to c1 after castling.");
        }

        board.ResetBoard();

        board.Grid[1, 7] = new(PieceType.Empty, new(1, 7));
        board.Grid[2, 7] = new(PieceType.Empty, new(2, 7));
        board.Grid[3, 7] = new(PieceType.Empty, new(3, 7));

        Move move2 = new(board.Grid[4, 7], board.Grid[2, 7]);
        board.MakeMove(move2);

        if (!board.Grid[3, 7].EqualsUncolored(PieceType.Rook))
        {
            Console.WriteLine("Black queenside castling failed! Rook did not move to d1 after king moved to c1.");
        }
        
        if (!board.Grid[2, 7].EqualsUncolored(PieceType.King))
        {
            Console.WriteLine("Black queenside castling failed! King did not move to c1 after castling.");
        }
    }
}