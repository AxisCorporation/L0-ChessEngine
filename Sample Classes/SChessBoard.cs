using System;
using CommunityToolkit.Mvvm.ComponentModel;
using L_0_Chess_Engine.Contracts;
using L_0_Chess_Engine.Models;

namespace L_0_Chess_Engine.SampleModels;

public class SChessBoard : IChessBoard
{
    public IChessPiece[,] Grid { get; set; } = new ChessPiece[8, 8];

    public bool IsCheck { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool IsCheckMate { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public SChessBoard()
    {
        for (int i = 0; i < Grid.GetLength(0); i++)
        {
            for (int j = 0; j < Grid.GetLength(1); j++)
            {
                Grid[i, j] = new ChessPiece(PieceType.Pawn | PieceType.White, new(i + 1, j + 1));
            }
        }
    }

    public void MakeMove(IMove move)
    {
        throw new NotImplementedException();
    }

    public void PrintBoardToTerminal()
    {
        throw new NotImplementedException();
    }

    public bool ReadFEN(string fen)
    {
        throw new NotImplementedException();
    }

    public void ResetBoard()
    {
        throw new NotImplementedException();
    }
}