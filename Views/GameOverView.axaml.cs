using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace L_0_Chess_Engine.Views;

public partial class GameOverView : Window
{
    public GameOverView(string text, MainWindow? mainWindow)
    {
        InitializeComponent();
        
        GameOverText.Text = text;

        Width = 400;
        Height = 500;
        CanResize = false;
        ShowInTaskbar = false;
        Title = "Game Over";

        MainMenuButton.Click += (_, _) =>
        {
            mainWindow!.SetMainContent(new MainMenuView(mainWindow));
            Close();
        };
    }
}