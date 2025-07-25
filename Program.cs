﻿using Avalonia;
using L_0_Chess_Engine.Tests;
using System;

namespace L_0_Chess_Engine;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.

    //test
    [STAThread]
    public static void Main(string[] args)
    {
        // Run Test Cases Here
        // AllTests.TestReadFENValid();

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
