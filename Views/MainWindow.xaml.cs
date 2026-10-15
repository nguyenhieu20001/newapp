using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using ExcavatorAngle.App.Controls;
using ExcavatorAngle.App.States;
using ExcavatorAngle.App.Views.Pages;

namespace ExcavatorAngle.App.Views;

/// <summary>
/// Code-behind của cửa sổ chính: chỉ chuyển sự kiện nút bấm sang controller
/// và đổi trang theo AppState.CurrentPage. Không có logic nghiệp vụ ở đây.
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainController _controller;
    private readonly AppState _state;
    private readonly MonitorPage _monitorPage;
    private readonly CalibrationPage _calibrationPage;

    public MainWindow(MainController controller, AppState state)
    {
        InitializeComponent();
        _controller = controller;
        _state = state;
        DataContext = state;

        _monitorPage = new MonitorPage { DataContext = state };
        _calibrationPage = new CalibrationPage(controller) { DataContext = state };

        state.PropertyChanged += OnStateChanged;
        ShowPage(state.CurrentPage);
        UpdateStatusDot();
    }

    private void OnStateChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppState.CurrentPage)) ShowPage(_state.CurrentPage);
        if (e.PropertyName == nameof(AppState.Connection)) UpdateStatusDot();
    }

    private void ShowPage(AppPage page) =>
        PageHost.Content = page == AppPage.Monitor ? _monitorPage : _calibrationPage;

    private void UpdateStatusDot() =>
        StatusDot.Fill = _state.Connection switch
        {
            ConnectionStatus.Receiving => Brushes.LimeGreen,
            ConnectionStatus.Waiting   => Brushes.Orange,
            _                          => Brushes.Gray
        };

    private void NavMonitor_Click(object sender, RoutedEventArgs e) => _controller.NavigateTo(AppPage.Monitor);
    private void NavCalibration_Click(object sender, RoutedEventArgs e) => _controller.NavigateTo(AppPage.Calibration);
    private void Start_Click(object sender, RoutedEventArgs e) => _controller.StartReceiving();
    private void Stop_Click(object sender, RoutedEventArgs e) => _controller.StopReceiving();
}
