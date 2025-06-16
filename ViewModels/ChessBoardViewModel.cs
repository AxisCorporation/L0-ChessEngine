using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
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
}