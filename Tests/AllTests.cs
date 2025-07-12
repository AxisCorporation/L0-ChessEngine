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