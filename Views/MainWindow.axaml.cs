using Avalonia.Controls;

namespace L_0_Chess_Engine.Views;

public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();

        var menuView = new MainMenuView(this);
        SetMainContent(menuView);
    }
    
    public void SetMainContent(UserControl control) => MainContent.Content = control;
    
}