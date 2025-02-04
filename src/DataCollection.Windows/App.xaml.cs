﻿using Microsoft.UI.Xaml;
using Windows.UI.Popups;
using DataCollection.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DataCollection.Windows;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App
{
    private MessageWindow? _messageWindow;
    private readonly ServiceProvider _services;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        DispatcherShutdownMode = DispatcherShutdownMode.OnExplicitShutdown;
        _services = new ServiceCollection()
            .AddDbContext<DataCollectorDbContext>()
            .AddScoped<IDataCollectionService, WindowsDataCollectionService>()
            .BuildServiceProvider();
        _services.GetRequiredService<DataCollectorDbContext>().Database.Migrate();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        UnhandledException += (_, eventArgs) =>
        {
            var window = new Window();
            var message = new MessageDialog(eventArgs.Message, "Exception occurred")
            {
                Options = MessageDialogOptions.AcceptUserInputAfterDelay,
                Commands =
                {
                    new UICommand("Exit", _ => Current.Exit()),
                    new UICommand("Cancel")
                },
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1
            };

            var handle = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(message, handle);

            message.ShowAsync();
        };

        _messageWindow = new MessageWindow(true, _services.CreateScope().ServiceProvider.GetRequiredService<IDataCollectionService>());
        _messageWindow?.StartDataCollection();
    }
}