using System;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using L_0_Chess_Engine.Enums;
using L_0_Chess_Engine.Models;

namespace L_0_Chess_Engine.ViewModels;

public partial class SquareViewModel : ObservableObject
{
    [ObservableProperty]
    private ChessPiece _piece;

    [ObservableProperty]
    private Bitmap _image;
    
    [ObservableProperty]
    private bool _isLightSquare;
    
    [ObservableProperty]
    private bool _isHighlighted;
    
    public ICommand? ClickCommand { get; set; }


    public SquareViewModel(ChessPiece piece)
    {
        Piece = piece;

        string uriStr = GetUriForPiece(Piece);
        Image = new (AssetLoader.Open(new Uri(uriStr)));
        
    }

    public SquareViewModel(PieceType type, Coordinate coordinate)
    {
        Piece = new ChessPiece(type, coordinate);

        string uriStr = GetUriForPiece(Piece);
        Image = new (AssetLoader.Open(new Uri(uriStr)));
    }

    public void UpdateImage()
    {
        string uriStr = GetUriForPiece(Piece);
        Image = new (AssetLoader.Open(new Uri(uriStr)));   
    }

    private static string GetUriForPiece(ChessPiece piece)
    {
        string uriStr = "avares://L-0 Chess Engine/" + (piece.Type switch
        {
            PieceType.Pawn | PieceType.White => "Assets/Images/W_Pawn.png",
            PieceType.Pawn | PieceType.Black => "Assets/Images/B_Pawn.png",

            PieceType.King | PieceType.White => "Assets/Images/W_King.png",
            PieceType.King | PieceType.Black => "Assets/Images/B_King.png",

            PieceType.Queen | PieceType.White => "Assets/Images/W_Queen.png",
            PieceType.Queen | PieceType.Black => "Assets/Images/B_Queen.png",

            PieceType.Knight | PieceType.White => "Assets/Images/W_Knight.png",
            PieceType.Knight | PieceType.Black => "Assets/Images/B_Knight.png",

            PieceType.Bishop | PieceType.White => "Assets/Images/W_Bishop.png",
            PieceType.Bishop | PieceType.Black => "Assets/Images/B_Bishop.png",

            PieceType.Rook | PieceType.White => "Assets/Images/W_Rook.png",
            PieceType.Rook | PieceType.Black => "Assets/Images/B_Rook.png",

            _ => @"Assets\Images\Empty.png"
        });

        return uriStr;
    }
    
}