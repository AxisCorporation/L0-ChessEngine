using Avalonia.Controls;
using L_0_Chess_Engine.Models;
using L_0_Chess_Engine.ViewModels;

namespace L_0_Chess_Engine.Views;

public partial class PawnPromotionView : UserControl
{
    public PawnPromotionView(bool isWhite)
    {
        InitializeComponent();
        
        DataContext = new PawnPromotionViewModel(isWhite);
    }
}