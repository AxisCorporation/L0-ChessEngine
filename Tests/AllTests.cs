using L_0_Chess_Engine.Models;
using System;

namespace L_0_Chess_Engine.Tests;

static class AllTests
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

    static void TestReadFENInvalidRow()
    {
        var board = ChessBoard.Instance;
        bool result = board.ReadFEN("8/8/8/8/8/8/8"); // only 7 rows

        if (result)
            throw new Exception("TestReadFENInvalidRow failed since should return false");

        Console.WriteLine("TestReadFENInvalidRow passed.");
    }

    static void TestReadFENInvalidCharacter()
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
    }
}