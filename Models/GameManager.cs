using L_0_Chess_Engine.Contracts;

namespace L_0_Chess_Engine.Models;

public class GameManager : IGameManager
{
    public IChessBoard Board { get; set; }
    public bool isWhiteTurn { get; private set; }
    public bool GameRunning { get; private set; }
    
    public GameManager()
    {
        Board = new ChessBoard();
        isWhiteTurn = true; // White always starts
        GameRunning = false;
    }

    public bool StartGame()
    {
        Board.ResetBoard();
        isWhiteTurn = true;
        GameRunning = true;
        return true;
    }

    public bool EndGame()
    {
        GameRunning = false;
        return true;
    }

    public void SwitchTurn()
    {
        isWhiteTurn = !isWhiteTurn;
    }

}

