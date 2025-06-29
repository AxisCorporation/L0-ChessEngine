using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Controls.Shapes;

using L_0_Chess_Engine.ViewModels;

namespace L_0_Chess_Engine.Views;

public partial class ChessBoardView : UserControl
{

    public ChessBoardView()
    {
        InitializeComponent();

        DataContext = new ChessBoardViewModel();

        
    }
}