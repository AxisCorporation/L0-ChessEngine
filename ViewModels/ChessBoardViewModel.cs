using L_0_Chess_Engine.Fake;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;

namespace L_0_Chess_Engine.ViewModels;

public partial class ChessBoardViewModel : ObservableObject
{
    [ObservableProperty]
    public int[,] board = new int[8, 8];

    ChessBoardViewModel(ChessBoard model)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                board[i, j] = (int) model.Grid[i, j].Type;
            }
        }
    }

} 