using Microsoft.UI.Xaml;

namespace DataCollection.Windows;

public sealed partial class MessageWindow : Window
{
    private readonly WindowMonitor _windowMonitor;
    private readonly WindowsDataCollector _dataCollector;

    public MessageWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = false;
        SetTitleBar(null);
        _dataCollector = new WindowsDataCollector();
        _windowMonitor = new WindowMonitor(_dataCollector);
    }

    public void StartDataCollection()
    {
        _windowMonitor.StartMonitoring();
    }

    ~MessageWindow()
    {
        _windowMonitor.StopMonitoring();
    }
}