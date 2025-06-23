using System;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using L_0_Chess_Engine.Models;

namespace L_0_Chess_Engine.ViewModels;

public partial class ChessPieceViewModel : ObservableObject
{
    [ObservableProperty]
    private ChessPiece _piece;

    [ObservableProperty]
    private Bitmap _image;


    public ChessPieceViewModel(ChessPiece piece)
    {
        Piece = piece;

        string UriStr = GetUriForPiece(Piece);
        Image = new(AssetLoader.Open(new Uri(UriStr)));
    }

    public ChessPieceViewModel(PieceType type)
    {
        Piece = new ChessPiece(type);

        string UriStr = GetUriForPiece(Piece);
        Image = new(AssetLoader.Open(new Uri(UriStr)));
    }

    public static string GetUriForPiece(ChessPiece piece)
    {
        string UriStr = "avares://L-0 Chess Engine/" + (piece.Type switch
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

        return UriStr;
    }
}