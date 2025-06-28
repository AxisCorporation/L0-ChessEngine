using System;
using L_0_Chess_Engine.Contracts;

namespace L_0_Chess_Engine.Models;

public class GameManager : IGameManager
{
    public IChessBoard Board { get; set; }
    public bool IsWhiteTurn { get; private set; }
    public bool GameRunning { get; private set; }

    public GameManager()
    {
        Board = new ChessBoard();
        IsWhiteTurn = true; // White always starts
        GameRunning = false;
    }

    public bool StartGame()
    {
        Board.ResetBoard();
        GameRunning = true;

        while (!Board.IsCheckMate)
        {
            /*
                TODO:
                1. Check who's turn it is, and make a new Move Object based on initial and destination coordinates 
                2. If move is valid, call Board.MakeMove() and switch turn 
                3. Do checks to see if Checkmate/Check has been achieved
            */
        }
        return true;
    }

    public bool EndGame()
    {
        GameRunning = false;
        return true;
    }

    public void SwitchTurn() => IsWhiteTurn = !IsWhiteTurn;
    

    public static ChessPiece GetPieceFromPromotion()
    {
        throw new NotSupportedException();
    }
}

