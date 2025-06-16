using System;
using L_0_Chess_Engine.Fake;
using CommunityToolkit.Mvvm.ComponentModel;
using L_0_Chess_Engine.Models;

namespace L_0_Chess_Engine.ViewModels;

public partial class ChessBoardViewModel : ObservableObject
{
    [ObservableProperty]
    public int[,] _board = new int[8, 8];

    public ChessBoardViewModel(ChessBoard model)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Board[i, j] = (int) model.Grid[i, j].Type;
            }
        }
    }
    
    public void ButtonClick()
    {
        Board[4, 4] = (int)(PieceType.Black | PieceType.Rook);
        Console.WriteLine("Button Clicked, Board Updated : " + Board[4, 4]);
    }

} 