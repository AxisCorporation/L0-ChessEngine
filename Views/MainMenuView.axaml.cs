using Avalonia.Interactivity;
using Avalonia.Controls;

namespace L_0_Chess_Engine.Views;

public partial class MainMenuView : UserControl
{
    public MainWindow? MainWindow { get; set; }

    public MainMenuView(MainWindow? mainWindow)
    {
        InitializeComponent();
        MainWindow = mainWindow;

        PlayToggle.Checked += (_, _) =>
        {
            PlayToggle.IsVisible = false; // Play itself gone
            PlayMainOptions.IsVisible = true;
            PlayMainOptions.Opacity = 1;

            OtherOptions.IsVisible = false; // hide credits/quit
        };

        LocalButton.Click += (_, _) =>
        {
            // hide Local/AI choices
            PlayMainOptions.IsVisible = false;
            PlayMainOptions.Opacity = 0;

            // show Local time options
            LocalTimeOptions.IsVisible = true;
            LocalTimeOptions.Opacity = 1;
        };

        AIButton.Click += (_, _) =>
        {
            // hide Local/AI choices
            PlayMainOptions.IsVisible = false;
            PlayMainOptions.Opacity = 0;

            // show AI time options
            AITimeOptions.IsVisible = true;
            AITimeOptions.Opacity = 1;
        };
    }

    public void PlayGame(object? sender, RoutedEventArgs e)
    {
        var chessBoard = new ChessBoardView();
        MainWindow?.SetMainContent(chessBoard);
    }
}