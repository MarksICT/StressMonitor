using Microsoft.UI.Xaml;

namespace DataCollection.Windows;

public sealed partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        this.SetWindowToMinimizeToTray();
    }

    private void myButton_Click(object sender, RoutedEventArgs e)
    {
        myButton.Content = "Clicked";
    }
}
