using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Photobooth.App;

public partial class MainWindow : Window
{
    /// <summary>
    /// Placeholder filter descriptors. Later entries can carry effect params / shader keys.
    /// </summary>
    private sealed record FilterOption(string Name);

    private readonly FilterOption[] _filters =
    [
        new("No Filter"),
        new("Sepia"),
        new("Black and White"),
    ];

    private readonly DispatcherTimer _filterNameHideTimer;

    private int _filterIndex;
    private Point? _swipeStart;
    private const double SwipeThresholdPx = 80;

    public MainWindow()
    {
        InitializeComponent();

        _filterNameHideTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(3),
        };
        _filterNameHideTimer.Tick += (_, _) =>
        {
            _filterNameHideTimer.Stop();
            FilterNameOverlay.Visibility = Visibility.Collapsed;
        };

        ApplySelectedFilter();
    }

    private void CaptureButton_Click(object sender, RoutedEventArgs e)
    {
        // Camera capture wires in next — stub keeps the touch path exercisable.
        StatusText.Text = "Capture coming soon…";
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        ExitApp();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        // Esc exits during development / operator recovery on a kiosk PC.
        if (e.Key == Key.Escape)
        {
            ExitApp();
        }
    }

    private void ExitApp()
    {
        Close();
    }

    private void PreviewArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _swipeStart = e.GetPosition(PreviewArea);
        PreviewArea.CaptureMouse();
    }

    private void PreviewArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_swipeStart is not Point start)
        {
            return;
        }

        var end = e.GetPosition(PreviewArea);
        TryCompleteSwipe(start, end);
        ReleaseSwipe();
    }

    private void PreviewArea_LostMouseCapture(object sender, MouseEventArgs e)
    {
        ReleaseSwipe();
    }

    private void TryCompleteSwipe(Point start, Point end)
    {
        var dx = end.X - start.X;
        var dy = end.Y - start.Y;

        // Prefer horizontal swipes; ignore mostly-vertical drags.
        if (Math.Abs(dx) < SwipeThresholdPx || Math.Abs(dx) < Math.Abs(dy))
        {
            return;
        }

        if (dx < 0)
        {
            CycleFilter(1);
        }
        else
        {
            CycleFilter(-1);
        }
    }

    private void ReleaseSwipe()
    {
        _swipeStart = null;
        if (PreviewArea.IsMouseCaptured)
        {
            PreviewArea.ReleaseMouseCapture();
        }
    }

    private void CycleFilter(int delta)
    {
        _filterIndex = (_filterIndex + delta + _filters.Length) % _filters.Length;
        ApplySelectedFilter();
    }

    private void ApplySelectedFilter()
    {
        var filter = _filters[_filterIndex];
        FilterNameText.Text = filter.Name;
        ShowFilterNameTemporarily();

        // Future: apply camera/preview effect from filter properties here.
    }

    private void ShowFilterNameTemporarily()
    {
        FilterNameOverlay.Visibility = Visibility.Visible;
        _filterNameHideTimer.Stop();
        _filterNameHideTimer.Start();
    }
}
