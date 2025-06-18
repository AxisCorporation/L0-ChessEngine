using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using L_0_Chess_Engine.Models;
using L_0_Chess_Engine.SampleModels;

namespace L_0_Chess_Engine.ViewModels;

public partial class ChessBoardViewModel : ObservableObject
{
    public ObservableCollection<ChessPieceViewModel> GridPieces { get; set; } = [];
    SChessBoard SampleBoard = new();

    public ChessBoardViewModel()
    {
        foreach (var Piece in SampleBoard.Grid)
        {
            GridPieces.Add(new ChessPieceViewModel((ChessPiece)Piece));    
        }
    }

    // For testing
    [RelayCommand]
    public void ClickMe()
    {
        for (int i = 0; i < 64; i++)
        {
            GridPieces[i] = new ChessPieceViewModel(PieceType.Rook | PieceType.White);
        }
    }
}