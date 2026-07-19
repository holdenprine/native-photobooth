using System.Windows;
using System.Windows.Input;

namespace Photobooth.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void CaptureButton_Click(object sender, RoutedEventArgs e)
    {
        // Camera capture wires in next — stub keeps the touch path exercisable.
        StatusText.Text = "Capture coming soon…";
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        // Esc exits during development / operator recovery on a kiosk PC.
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }
}
