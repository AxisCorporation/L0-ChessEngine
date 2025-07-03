using L_0_Chess_Engine.Models;
using System;

namespace L_0_Chess_Engine.Tests;

class AllTests
{

    static void TestReadFENValid()
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

}