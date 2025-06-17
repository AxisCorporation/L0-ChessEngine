using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using L_0_Chess_Engine.Models;
using L_0_Chess_Engine.ViewModels;
using L_0_Chess_Engine.Views;

namespace L_0_Chess_Engine;

public partial class App : Application
{
    public override void Initialize()
    {

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow()
            {
                DataContext = new ChessBoardViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}