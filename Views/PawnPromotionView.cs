using Avalonia.Controls;
using L_0_Chess_Engine.Models;
using L_0_Chess_Engine.ViewModels;

namespace L_0_Chess_Engine.Views;

public partial class PawnPromotionView : Window
{
    public PawnPromotionView(bool isWhite)
    {
        InitializeComponent();

        Width = 300;
        Height = 150;
        CanResize = false;
        ShowInTaskbar = false;
        Title = "Pawn Promotion";
        
        DataContext = new PawnPromotionViewModel(isWhite);
    }
}