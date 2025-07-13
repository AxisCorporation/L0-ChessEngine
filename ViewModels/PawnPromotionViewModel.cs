using System;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using L_0_Chess_Engine.Models;

namespace L_0_Chess_Engine.ViewModels;

public partial class PawnPromotionViewModel : ObservableObject
{
    private readonly bool _isWhite;
    private readonly PieceType _colorFlag;
    
    [ObservableProperty]
    private Bitmap _queenImage;
    
    [ObservableProperty]
    private Bitmap _rookImage;
    
    [ObservableProperty]
    private Bitmap _bishopImage;
    
    [ObservableProperty]
    private Bitmap _knightImage;
    
    public ICommand SelectQueenCommand { get; }
    public ICommand SelectRookCommand { get; }
    public ICommand SelectBishopCommand { get; }
    public ICommand SelectKnightCommand { get; }
    
    public event Action<PieceType>? PieceSelected;
    
    public PawnPromotionViewModel(bool isWhite)
    {
        _isWhite = isWhite;
        _colorFlag = isWhite ? PieceType.White : PieceType.Black;
        
        string colorPrefix = _isWhite ? "W" : "B";
        
        QueenImage = new Bitmap(AssetLoader.Open(new Uri($"avares://L-0 Chess Engine/Assets/Images/{colorPrefix}_Queen.png")));
        RookImage = new Bitmap(AssetLoader.Open(new Uri($"avares://L-0 Chess Engine/Assets/Images/{colorPrefix}_Rook.png")));
        BishopImage = new Bitmap(AssetLoader.Open(new Uri($"avares://L-0 Chess Engine/Assets/Images/{colorPrefix}_Bishop.png")));
        KnightImage = new Bitmap(AssetLoader.Open(new Uri($"avares://L-0 Chess Engine/Assets/Images/{colorPrefix}_Knight.png")));
        
        SelectQueenCommand = new RelayCommand(() => SelectPiece(PieceType.Queen | _colorFlag));
        SelectRookCommand = new RelayCommand(() => SelectPiece(PieceType.Rook | _colorFlag));
        SelectBishopCommand = new RelayCommand(() => SelectPiece(PieceType.Bishop | _colorFlag));
        SelectKnightCommand = new RelayCommand(() => SelectPiece(PieceType.Knight | _colorFlag));
    }
    
    private void SelectPiece(PieceType pieceType)
    {
        ChessBoard.SetPromotedPieceType(pieceType);
        PieceSelected?.Invoke(pieceType);
    }
}